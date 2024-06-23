using System;

namespace ContractBridge.Core.Impl
{
    public class TurnPlayContext : ITurnPlayContext
    {
        public TurnPlayContext(ITurnSequence turnSequence)
        {
            TurnSequence = turnSequence;
        }

        public void PlayTurn(Seat seat, Action action)
        {
            if (TurnSequence.NextTurn() is not { } turn)
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

        public ITurnSequence TurnSequence { get; }

        public bool CanPlayTurn(Seat seat, Func<bool> pred)
        {
            if (TurnSequence.NextTurn() is not { } turn)
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