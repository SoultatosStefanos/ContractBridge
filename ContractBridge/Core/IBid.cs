using System;

namespace ContractBridge.Core
{
    public class BidAlreadyReDoubledException : Exception
    {
    }

    public interface IBid
    {
        Level Level { get; }

        Denomination Denomination { get; }

        Seat? DoubledSeat();

        bool IsDoubled();

        bool IsRedoubled();

        void Double(Seat fromSeat);

        event EventHandler Doubled;

        event EventHandler Redoubled;
    }

    public static class BidExtensions
    {
        public static int Tricks(this IBid bid)
        {
            return (int)bid.Level + 6;
        }
    }
}