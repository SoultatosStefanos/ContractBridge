using System;

namespace ContractBridge.Core.Impl
{
    public class Turn : ITurn
    {
        private bool _played;

        public Turn(Seat seat)
        {
            Seat = seat;
        }

        public Seat Seat { get; }

        public bool IsPlayed()
        {
            return _played;
        }

        public void MarkPlayed()
        {
            if (IsPlayed())
            {
                throw new TurnAlreadyPlayedException();
            }

            _played = true;

            RaiseMarkedPlayedEvent();
        }

        public event EventHandler? MarkedPlayed;

        private void RaiseMarkedPlayedEvent()
        {
            MarkedPlayed?.Invoke(this, EventArgs.Empty);
        }
    }
}