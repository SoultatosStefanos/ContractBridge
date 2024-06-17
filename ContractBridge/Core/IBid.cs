using System;

namespace ContractBridge.Core
{
    public interface IBid
    {
        byte Level { get; }

        Denomination Denomination { get; }
    }

    public static class BidExtensions
    {
        public static int Tricks(this IBid bid)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}