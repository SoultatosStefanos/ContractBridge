using System;

namespace ContractBridge.Core
{
    public enum Seat : byte
    {
        North,
        East,
        South,
        West
    }

    public static class SeatExtensions
    {
        public static Seat NextSeat(this Seat seat)
        {
            throw new NotImplementedException(); // TODO
        }

        public static Seat Partner(this Seat seat)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}