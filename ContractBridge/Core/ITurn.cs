using System;

namespace ContractBridge.Core
{
    public class TurnAlreadyPlayedException : Exception
    {
    }

    public interface ITurn
    {
        Seat Seat { get; }

        bool IsPlayed();

        void MarkPlayed();

        event EventHandler MarkedPlayed;
    }
}