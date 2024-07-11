using System;
using ContractBridge.Core;

namespace ContractBridge.Solver.Impl
{
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
}