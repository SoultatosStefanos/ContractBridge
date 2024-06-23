using System;

namespace ContractBridge.Core
{
    public class PlayOutOfTurnException : Exception
    {
    }

    public interface ITurnPlayContext
    {
        ITurnSequence TurnSequence { get; }

        bool CanPlayTurn(Seat seat, Func<bool> pred);

        void PlayTurn(Seat seat, Action action);
    }
}