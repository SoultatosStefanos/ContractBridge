using System;

namespace ContractBridge.Core
{
    public enum Partnership : byte
    {
        NorthSouth = 0,
        EastWest = 1
    }

    public static class PartnershipExtensions
    {
        public static Partnership Partnership(this Seat seat)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}