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
            return seat switch
            {
                Seat.East or Seat.West => Core.Partnership.EastWest,
                Seat.North or Seat.South => Core.Partnership.NorthSouth,
                _ => throw new ArgumentOutOfRangeException(nameof(seat), seat, null)
            };
        }
    }
}