using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ContractBridge.Core;
using ContractBridge.Core.Impl;

namespace ContractBridge.Solver.Impl
{
    #region DdNative

    [StructLayout(LayoutKind.Sequential)]
    internal struct DdDeal
    {
        public int trump; // Spades=0, Hearts=1, Diamonds=2, Clubs=3,  NT=4

        public int first; // 0=North, 1=East, 2=South, 3=West, leading hand for the trick.

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] currentTrickSuit; // Initialize with -1 if no cards played yet

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] currentTrickRank; // Initialize with -1 if no cards played yet

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public uint[] remainCards; // 4x4. 1st index hand (0-3), 2nd index suit (0-3), ...
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DdFutureTricks
    {
        public int nodes; // Number of searched nodes

        public int cards; // No. of alternative cards

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public int[] suit; // 0=Spades, 1=Hearts, 2=Diamonds, 3=Clubs

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public int[] rank; // 2-14 for 2 through Ace

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public int[] equals; // Bit string of ranks for equivalent lower rank cards.

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public int[] score; // -1 for target not reached, otherwise target or max no of tricks.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DdTableDealPbn
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public char[] cards;

        public DdTableDealPbn(string pbnCards)
        {
            cards = new char[80];
            for (var i = 0; i < pbnCards.Length; i++)
                cards[i] = pbnCards[i];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DdTableResults
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public int[] solution;
    }

    internal class DdsImport
    {
#if WINDOWS
    [DllImport("dds.dll")]
#else
        [DllImport("libdds.so")]
#endif
        public static extern int SolveBoard(
            [In] DdDeal ddDeal, // struct deal dl, 
            int target, // -1 to solve for all possible tricks
            int solutions, // 3 = all solutions
            int mode, // Mode (0 = normal)
            [In] [Out] ref DdFutureTricks ddFutureTricks, // struct futureTricks *futp
            int threadIndex // 0-15 (0 for single-threaded)
        );

#if WINDOWS
    [DllImport("dds.dll")]
#else
        [DllImport("libdds.so")]
#endif
        public static extern int CalcDDtablePBN(
            [In] DdTableDealPbn deal,
            [In] [Out] ref DdTableResults results
        );
    }

    #endregion

    internal class BoHaglundDoubleDummySolution : IDoubleDummySolution
    {
        private readonly IEnumerable<IContract> _contracts;

        private readonly IDictionary<IContract, IEnumerable<ICard>> _plays;

        public BoHaglundDoubleDummySolution(
            IEnumerable<IContract> contracts,
            IDictionary<IContract, IEnumerable<ICard>> plays
        )
        {
            _contracts = contracts;
            _plays = plays;
        }

        public IContract? MakeableContract(Seat declarer, Denomination denomination)
        {
            return _contracts.FirstOrDefault(c => c.Declarer == declarer && c.Denomination == denomination);
        }

        public IEnumerable<IContract> MakeableContracts(Seat declarer)
        {
            return _contracts.Where(c => c.Declarer == declarer);
        }

        public IEnumerable<ICard> OptimalPlays(IContract contract)
        {
            return _plays.TryGetValue(contract, out var play) ? play : Enumerable.Empty<ICard>();
        }
    }

    public class BoHaglundDoubleDummySolver : IDoubleDummySolver
    {
        private readonly IContractFactory _contractFactory;

        public BoHaglundDoubleDummySolver(IContractFactory contractFactory)
        {
            _contractFactory = contractFactory;
        }

        public IDoubleDummySolution Analyze(ISession session)
        {
            if (session.Phase < Phase.Auction)
            {
                throw new InvalidPhaseForSolving();
            }

            var makeableContracts = CalculateMakeableContracts(session.Board);
            var optimalPlays = new Dictionary<IContract, IEnumerable<ICard>>();

            var enumerableMakeableContracts = makeableContracts as IContract[] ?? makeableContracts.ToArray();

            if (session.Phase != Phase.Play)
            {
                return new BoHaglundDoubleDummySolution(enumerableMakeableContracts, optimalPlays);
            }

            foreach (var contract in enumerableMakeableContracts)
            {
                optimalPlays[contract] = CalculateOptimalPlays(session, contract);
            }

            return new BoHaglundDoubleDummySolution(enumerableMakeableContracts, optimalPlays);
        }

        private static int StrainToInt(Denomination strain)
        {
            return strain != Denomination.NoTrumps ? 4 - (int)strain : 4;
        }

        private static int SuitToInt(Suit suit)
        {
            return 4 - (int)suit;
        }

        private static Suit IntToSuit(int suit)
        {
            return (Suit)(4 + suit);
        }

        private static IEnumerable<T> EnumValues<T>(Type enumType)
        {
            return (T[])Enum.GetValues(enumType);
        }

        private IEnumerable<IContract> CalculateMakeableContracts(IBoard board)
        {
            var table = new DdTableDealPbn(board.ToPbn());

            var results = new DdTableResults
            {
                solution = new int[20]
            };

            var status = DdsImport.CalcDDtablePBN(table, ref results);
            if (status < 0)
            {
                throw new DoubleDummySolverException($"CalcDDtablePBN failed with status {status}");
            }

            return DetermineMakeableContracts();

            IEnumerable<IContract> DetermineMakeableContracts()
            {
                var next = 0;
                return (from strain in EnumValues<Denomination>(typeof(Denomination))
                    from declarer in EnumValues<Seat>(typeof(Seat))
                    let tricks = results.solution[next++]
                    where tricks > 6
                    select _contractFactory.Create((Level)tricks - 6, strain, declarer, null)).ToList();
            }
        }

        private static IEnumerable<ICard> CalculateOptimalPlays(ISession session, IContract contract)
        {
            var board = session.Board;
            var game = session.Game!;

            const int target = -1; // all possible tricks
            const int solutions = 3; // all solutions
            const int mode = 0; // normal
            const int threadIndex = 0; // single threaded

            var currentDeal = new DdDeal
            {
                trump = StrainToInt(contract.Denomination),
                first = (int)(Seat)game.Lead!,
                currentTrickSuit = new[]
                {
                    CurrentTrickSuitToBit(0),
                    CurrentTrickSuitToBit(1),
                    CurrentTrickSuitToBit(2)
                },
                currentTrickRank = new[]
                {
                    CurrentTrickRankToBit(0),
                    CurrentTrickRankToBit(1),
                    CurrentTrickRankToBit(2)
                },
                remainCards = HandsTo4X4Matrix(board)
            };

            var futureTricks = new DdFutureTricks
            {
                suit = new int[13],
                rank = new int[13],
                equals = new int[13],
                score = new int[13]
            };

            var status = DdsImport.SolveBoard(currentDeal, target, solutions, mode, ref futureTricks, threadIndex);
            if (status != 1)
            {
                throw new DoubleDummySolverException($"SolveBoard failed with status {status}");
            }

            return DetermineOptimalPlays();

            int CurrentTrickSuitToBit(int index)
            {
                var playedCards = game.PlayedCards;
                var card = playedCards.ElementAtOrDefault(index);
                return card != null ? SuitToInt(card.Suit) : -1;
            }

            int CurrentTrickRankToBit(int index)
            {
                var playedCards = game.PlayedCards;
                var card = playedCards.ElementAtOrDefault(index);
                return card != null ? (int)card.Rank : -1;
            }

            IEnumerable<ICard> DetermineOptimalPlays()
            {
                var optimalPlays = new List<ICard>();

                for (var i = 0; i < futureTricks.cards; ++i)
                {
                    var suit = IntToSuit(futureTricks.suit[i]);
                    var rank = (Rank)futureTricks.rank[i];

                    optimalPlays.Add(session.Deck[rank, suit]);
                }

                return optimalPlays;
            }
        }

        private static uint[] HandsTo4X4Matrix(IBoard board)
        {
            var remainingCards = new uint[16];

            foreach (var seat in EnumValues<Seat>(typeof(Seat)))
            {
                var hand = board.Hand(seat);

                var handIndex = (int)seat;

                foreach (var card in hand)
                {
                    var suitIndex = SuitToInt(card.Suit);
                    var rankBitPosition = (int)card.Rank - 2;

                    remainingCards[handIndex * 4 + suitIndex] |= 1U << rankBitPosition;
                }
            }

            return remainingCards;
        }
    }
}