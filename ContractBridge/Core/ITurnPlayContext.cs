using System;

namespace ContractBridge.Core
{
    public class PlayOutOfTurnException : Exception
    {
    }

    public interface ITurnPlayContext
    {
        bool CanPlayTurn(Seat seat, Func<bool> pred);

        void PlayTurn(Seat seat, Action action);
    }
}