using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ContractBridge.Core;
using ContractBridge.Core.Impl;

// TODO Test enumeration conversions

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
        public int[] score; // -1 for target not reaached, otherwise target or max no of tricks.
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
        [DllImport("dds.dll")]
        public static extern int SolveBoard(
            [In] DdDeal ddDeal, // struct deal dl, 
            int target, // -1 to solve for all possible tricks
            int solutions, // 3 = all solutions
            int mode, // Mode (0 = normal)
            [In] [Out] ref DdFutureTricks ddFutureTricks, // struct futureTricks *futp
            int threadIndex // 0-15 (0 for single-threaded)
        );

        [DllImport("dds.dll")]
        public static extern int CalcDDtablePBN(
            [In] DdTableDealPbn deal,
            [In] [Out] ref DdTableResults results
        );
    }

    #endregion
    
    internal class BoHaglundDoubleDummySolution : IDoubleDummySolution
    {
        private readonly IEnumerable<IContract> _makeableContracts;

        private readonly IDictionary<IContract, IEnumerable<ICard>> _optimalPlays;

        public BoHaglundDoubleDummySolution(
            IEnumerable<IContract> makeableContracts,
            IDictionary<IContract, IEnumerable<ICard>> optimalPlays
        )
        {
            _makeableContracts = makeableContracts;
            _optimalPlays = optimalPlays;
        }

        public IContract? MakeableContract(Seat declarer, Denomination denomination)
        {
            return _makeableContracts.FirstOrDefault(c => c.Declarer == declarer && c.Denomination == denomination);
        }

        public IEnumerable<IContract> MakeableContracts(Seat declarer)
        {
            return _makeableContracts.Where(c => c.Declarer == declarer);
        }

        public IEnumerable<ICard> OptimalPlays(IContract contract)
        {
            return _optimalPlays.TryGetValue(contract, out var play) ? play : Enumerable.Empty<ICard>();
        }
    }

    public class BoHaglundDoubleDummySolver : IDoubleDummySolver
    {
        private static readonly Denomination[] StrainOrder =
        {
            Denomination.Spades, Denomination.Hearts,
            Denomination.Diamonds, Denomination.Clubs,
            Denomination.NoTrumps
        };

        private static readonly Seat[] SeatOrder =
        {
            Seat.North, Seat.East, Seat.South, Seat.West
        };

        private readonly IContractFactory _contractFactory;

        public BoHaglundDoubleDummySolver(IContractFactory contractFactory)
        {
            _contractFactory = contractFactory;
        }

        public IDoubleDummySolution Analyze(ISession session)
        {
            if (session.Game == null)
            {
                throw new InvalidPhaseForSolving();
            }

            var makeableContracts = CalculateMakeableContracts(session.Game!);
            var optimalPlays = new Dictionary<IContract, IEnumerable<ICard>>();

            var enumerableMakeableContracts = makeableContracts as IContract[] ?? makeableContracts.ToArray();
            foreach (var contract in enumerableMakeableContracts)
            {
                optimalPlays[contract] = CalculateOptimalPlays(session, contract);
            }

            return new BoHaglundDoubleDummySolution(enumerableMakeableContracts, optimalPlays);
        }

        private IEnumerable<IContract> CalculateMakeableContracts(IGame game)
        {
            var table = new DdTableDealPbn(game.Board.HandsToPbn());

            var results = new DdTableResults
            {
                solution = new int[20]
            };

            var status = DdsImport.CalcDDtablePBN(table, ref results);
            if (status < 0)
            {
                throw new DoubleDummySolverException($"CalcDDtablePBN failed with status {status}");
            }

            var next = 0;
            var makeableContracts = new List<IContract>();
            foreach (var strain in StrainOrder)
            {
                makeableContracts.AddRange(
                    from declarer in SeatOrder
                    let tricks = results.solution[next++]
                    where tricks > 0
                    select _contractFactory.Create((Level)tricks - 6, strain, declarer, null)
                );
            }

            return makeableContracts;
        }

        private IEnumerable<ICard> CalculateOptimalPlays(ISession session, IContract contract)
        {
            var target = -1; // all possible tricks
            var solutions = 3; // all solutions
            var mode = 0; // normal
            var threadIndex = 0; // single threaded

            var currentDeal = new DdDeal
            {
                trump = (int)contract.Denomination,
                first = (int)session.Game!.Board.Dealer!, // TODO Is this the dealer or the declarer???
                currentTrickSuit = new int[3] { -1, -1, -1 }, // TODO
                currentTrickRank = new int[3] { -1, -1, -1 }, // TODO
                remainCards = GetRemainingCards(session.Board)
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

            var optimalPlays = new List<ICard>();
            for (var i = 0; i < futureTricks.cards; ++i)
            {
                var suit = (Suit)futureTricks.suit[i];
                var rank = (Rank)futureTricks.rank[i];
                optimalPlays.Add(session.Deck[rank, suit]);
            }

            return optimalPlays;
        }

        private static uint[] GetRemainingCards(IBoard board)
        {
            var remainingCards = new uint[16];

            foreach (var seat in SeatOrder)
            {
                var hand = board.Hand(seat);

                var handIndex = (int)seat;

                foreach (var card in hand)
                {
                    var suitIndex = (int)card.Suit;
                    var rankBitPosition = (int)card.Rank - 2;

                    remainingCards[handIndex * 4 + suitIndex] |= 1U << rankBitPosition;
                }
            }

            return remainingCards;
        }
    }
}