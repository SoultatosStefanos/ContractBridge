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

        public Bid(Level level, Denomination denomination)
        {
            Level = level;
            Denomination = denomination;
        }

        public Level Level { get; }

        public Denomination Denomination { get; }

        public bool IsDoubled()
        {
            return _bidState == BidState.Doubled;
        }

        public bool IsRedoubled()
        {
            return _bidState == BidState.Redoubled;
        }

        public void Double()
        {
            switch (_bidState)
            {
                case BidState.Normal:
                    _bidState = BidState.Doubled;
                    RaiseDoubledEvent();
                    break;

                case BidState.Doubled:
                    _bidState = BidState.Redoubled;
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