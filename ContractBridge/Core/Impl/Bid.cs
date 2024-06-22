using System;

namespace ContractBridge.Core.Impl
{
    internal enum BidState
    {
        Normal,
        Doubled,
        Redoubled
    }

    public class Bid : IBid
    {
        private BidState _bidState = BidState.Normal;
        private Seat? _doubledSeat;

        public Bid(Level level, Denomination denomination)
        {
            Level = level;
            Denomination = denomination;
        }

        public Level Level { get; }

        public Denomination Denomination { get; }

        public Seat? DoubledSeat()
        {
            return _doubledSeat;
        }

        public bool IsDoubled()
        {
            return _bidState == BidState.Doubled;
        }

        public bool IsRedoubled()
        {
            return _bidState == BidState.Redoubled;
        }

        public void Double(Seat fromSeat)
        {
            switch (_bidState)
            {
                case BidState.Normal:
                    _bidState = BidState.Doubled;
                    _doubledSeat = fromSeat;
                    RaiseDoubledEvent();
                    break;

                case BidState.Doubled:
                    _bidState = BidState.Redoubled;
                    _doubledSeat = fromSeat;
                    RaiseRedoubledEvent();
                    break;

                case BidState.Redoubled:
                    throw new BidAlreadyReDoubledException();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public event EventHandler? Doubled;
        public event EventHandler? Redoubled;

        private void RaiseRedoubledEvent()
        {
            Redoubled?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseDoubledEvent()
        {
            Doubled?.Invoke(this, EventArgs.Empty);
        }
    }
}