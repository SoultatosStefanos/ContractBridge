using System;

namespace ContractBridge.Core.Impl
{
    public class TurnPlayContext : ITurnPlayContext
    {
        private readonly ITurnSequence _turnSequence;

        public TurnPlayContext(ITurnSequence turnSequence)
        {
            _turnSequence = turnSequence;
        }

        public void PlayTurn(Seat seat, Action action)
        {
            if (_turnSequence.NextTurn() is not { } turn)
            {
                throw new PlayOutOfTurnException();
            }

            if (turn.Seat != seat)
            {
                throw new PlayOutOfTurnException();
            }

            action.Invoke();
            turn.MarkPlayed();
        }

        public bool CanPlayTurn(Seat seat, Func<bool> pred)
        {
            if (_turnSequence.NextTurn() is not { } turn)
            {
                return false;
            }

            if (turn.Seat == seat)
            {
                return !turn.IsPlayed() && pred.Invoke();
            }

            return false;
        }
    }
}