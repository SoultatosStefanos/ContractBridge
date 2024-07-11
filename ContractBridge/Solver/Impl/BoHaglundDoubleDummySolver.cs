using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ContractBridge.Core;
using ContractBridge.Core.Impl;

namespace ContractBridge.Solver.Impl
{
    #region DdNativeHelpers

    internal class DdsHelper
    {
        internal static char[] PbnToChars(string pbn)
        {
            var result = new char[80];
            for (var i = 0; i < pbn.Length; i++)
                result[i] = pbn[i];

            return result;
        }
    }

    internal static class EnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
        {
            return self.Select((item, index) => (item, index));
        }
    }

    internal static class DdsExtensions
    {
        internal static int ToDdsOrder(this Suit suit)
        {
            return suit switch
            {
                Suit.Clubs => 3,
                Suit.Diamonds => 2,
                Suit.Hearts => 1,
                Suit.Spades => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(suit), suit, null)
            };
        }

        internal static int ToDdsOrder(this Denomination denomination)
        {
            return denomination switch
            {
                Denomination.Clubs => 3,
                Denomination.Diamonds => 2,
                Denomination.Hearts => 1,
                Denomination.Spades => 0,
                Denomination.NoTrumps => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(denomination), denomination, null)
            };
        }

        internal static int ToDdsOrder(this Seat seat)
        {
            return seat switch
            {
                Seat.North => 0,
                Seat.East => 1,
                Seat.South => 2,
                Seat.West => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(seat), seat, null)
            };
        }

        internal static int ToDdsOrder(this Rank rank)
        {
            return (int)rank;
        }

        internal static Suit ToSuit(this int ddsOrder)
        {
            return ddsOrder switch
            {
                0 => Suit.Spades,
                1 => Suit.Hearts,
                2 => Suit.Diamonds,
                3 => Suit.Clubs,
                _ => throw new ArgumentOutOfRangeException(nameof(ddsOrder), ddsOrder, null)
            };
        }

        internal static Rank ToRank(this int ddsOrder)
        {
            return (Rank)ddsOrder;
        }
    }

    #endregion

    #region DdNative

    [StructLayout(LayoutKind.Sequential)]
    internal struct DdTableDealPbn
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public char[] cards;

        public DdTableDealPbn(string pbnCards)
        {
            cards = DdsHelper.PbnToChars(pbnCards);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DdDealPbn
    {
        public int trump; // Spades=0, Hearts=1, Diamonds=2, Clubs=3,  NT=4

        public int first; // 0=North, 1=East, 2=South, 3=West, leading hand for the trick.

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] currentTrickSuit; // Initialize with 0 if no cards played yet

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] currentTrickRank; // Initialize with 0 if no cards played yet

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public char[] remainCards;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DdFutureTricks
    {
        [MarshalAs(UnmanagedType.I4)]
        public int nodes; // Number of searched nodes

        [MarshalAs(UnmanagedType.I4)]
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
        public static extern int SolveBoardPBN(
            [In] DdDealPbn ddDeal, // struct deal dl, 
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

    #region Solutions

    internal class BoHaglundDoubleDummyContractsSolution : IDoubleDummyContractsSolution
    {
        private readonly IEnumerable<IContract> _contracts;

        public BoHaglundDoubleDummyContractsSolution(IEnumerable<IContract> contracts)
        {
            _contracts = contracts;
        }

        public IContract? MakeableContract(Seat declarer, Denomination denomination)
        {
            return _contracts.FirstOrDefault(c => c.Declarer == declarer && c.Denomination == denomination);
        }

        public IEnumerable<IContract> MakeableContracts(Seat declarer)
        {
            return _contracts.Where(c => c.Declarer == declarer);
        }
    }

    internal class BoHaglundDoubleDummyPlaysSolution : IDoubleDummyPlaysSolution
    {
        private readonly IDictionary<Seat, IEnumerable<ICard>> _playsBySeat;

        public BoHaglundDoubleDummyPlaysSolution(IDictionary<Seat, IEnumerable<ICard>> playsBySeat)
        {
            _playsBySeat = playsBySeat;
        }

        public IEnumerable<ICard> OptimalPlays(Seat seat)
        {
            return _playsBySeat.TryGetValue(seat, out var play) ? play : Enumerable.Empty<ICard>();
        }
    }

    #endregion

    #region Solver

    public class BoHaglundDoubleDummySolver : IDoubleDummySolver
    {
        private readonly IContractFactory _contractFactory;

        public BoHaglundDoubleDummySolver(IContractFactory contractFactory)
        {
            _contractFactory = contractFactory;
        }

        public IDoubleDummyContractsSolution AnalyzeContracts(ISession session)
        {
            if (session.Phase < Phase.Auction)
            {
                throw new InvalidPhaseForSolving();
            }

            var makeableContracts = CalculateMakeableContracts(session.Board);

            return new BoHaglundDoubleDummyContractsSolution(
                makeableContracts as IContract[] ?? makeableContracts.ToArray()
            );
        }

        public IDoubleDummyPlaysSolution AnalyzePlays(ISession session, IContract contract)
        {
            if (session.Phase < Phase.Auction)
            {
                throw new InvalidPhaseForSolving();
            }

            var optimalPlaysBySeat = new Dictionary<Seat, IEnumerable<ICard>>();

            var optimalPlays = CalculateOptimalPlays(session, contract);

            foreach (var seat in EnumValues<Seat>(typeof(Seat)))
            {
                // ReSharper disable once PossibleMultipleEnumeration
                optimalPlaysBySeat[seat] = optimalPlays.Where(card => session.Board.Hand(seat).Contains(card)).ToList();
            }

            return new BoHaglundDoubleDummyPlaysSolution(optimalPlaysBySeat);
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

            var trump = contract.Denomination.ToDdsOrder();
            var first = game.Lead!.Value.ToDdsOrder();

            var currentTrickSuit = new int[3];
            var currentTrickRank = new int[3];
            foreach (var (trick, i) in game.PlayedCards.WithIndex())
            {
                currentTrickRank[i] = trick.Rank.ToDdsOrder();
                currentTrickSuit[i] = trick.Suit.ToDdsOrder();
            }

            var remainingCards = DdsHelper.PbnToChars(board.ToPbn());

            var currentDeal = new DdDealPbn
            {
                trump = trump,
                first = first,
                currentTrickSuit = currentTrickSuit,
                currentTrickRank = currentTrickRank,
                remainCards = remainingCards
            };

            var futureTricks = new DdFutureTricks();

            var status = DdsImport.SolveBoardPBN(currentDeal, target, solutions, mode, ref futureTricks, threadIndex);
            if (status != 1)
            {
                throw new DoubleDummySolverException($"SolveBoard failed with status {status}");
            }

            return DetermineOptimalPlays();

            IEnumerable<ICard> DetermineOptimalPlays()
            {
                var optimalPlays = new List<ICard>();

                for (var i = 0; i < futureTricks.cards; ++i)
                {
                    if (futureTricks.rank[i] == 0) // No optimal plays.
                    {
                        break;
                    }

                    var rank = futureTricks.rank[i].ToRank();
                    var suit = futureTricks.suit[i].ToSuit();

                    optimalPlays.Add(session.Deck[rank, suit]);
                }

                return optimalPlays;
            }
        }
    }

    #endregion
}