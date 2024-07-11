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

    public static class DdsSeatExtensions
    {
        public static Seat NextSeat(this Seat seat)
        {
            return (Seat)((int)(seat + 1) % SeatCount());
        }

        public static Seat Partner(this Seat seat)
        {
            return (Seat)((int)(seat + 2) % SeatCount());
        }

        private static int SeatCount()
        {
            return Enum.GetNames(typeof(Seat)).Length;
        }
    }
}