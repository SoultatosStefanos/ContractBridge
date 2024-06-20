using System;
using System.Diagnostics;

namespace ContractBridge.Core.Impl
{
    public class TurnSequence : ITurnSequence
    {
        private readonly ITurnFactory _turnFactory;

        private Seat? _lead;

        private ITurn? _nextTurn;

        public TurnSequence(ITurnFactory turnFactory)
        {
            _turnFactory = turnFactory;
        }

        public Seat? Lead
        {
            get => _lead;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Lead cannot be null.");
                }

                _lead = value;

                RaiseLeadSetEvent(_lead.Value);

                _nextTurn = MakeNextTurn(_lead.Value);

                RaiseTurnChangedEvent(_nextTurn);
            }
        }

        public ITurn? NextTurn()
        {
            return _nextTurn;
        }

        public void Restart()
        {
            _lead = null;
            _nextTurn = null;

            RaiseRestartedEvent();
        }

        public event EventHandler<ITurnSequence.LeadEventArgs>? LeadSet;
        public event EventHandler<ITurnSequence.TurnEventArgs>? TurnChanged;
        public event EventHandler? Restarted;

        private ITurn MakeNextTurn(Seat turnSeat)
        {
            var newTurn = _turnFactory.Create(turnSeat);
            newTurn.MarkedPlayed += OnTurnMarkedPlayed;
            return newTurn;
        }

        private void RaiseLeadSetEvent(Seat lead)
        {
            LeadSet?.Invoke(this, new ITurnSequence.LeadEventArgs(lead));
        }

        private void RaiseTurnChangedEvent(ITurn turn)
        {
            TurnChanged?.Invoke(this, new ITurnSequence.TurnEventArgs(turn));
        }

        private void RaiseRestartedEvent()
        {
            Restarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnTurnMarkedPlayed(object sender, EventArgs args)
        {
            if (_nextTurn is not { } nextTurnValue) return;

            var turn = (ITurn)sender;
            Debug.Assert(nextTurnValue.Seat == turn.Seat);

            var nextTurnSeat = turn.Seat.NextSeat();
            _nextTurn = MakeNextTurn(nextTurnSeat);

            RaiseTurnChangedEvent(_nextTurn);
        }
    }
}