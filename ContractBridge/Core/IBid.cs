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

        bool IsDoubled();

        bool IsRedoubled();

        void Double();

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