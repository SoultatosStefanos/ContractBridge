using System;

namespace ContractBridge.Core
{
    public interface ITurn
    {
        Seat Seat { get; }

        bool IsPlayed();

        void MarkPlayed();

        event EventHandler<PlayedEventArgs> MarkedPlayed;

        public sealed class PlayedEventArgs : EventArgs
        {
            public PlayedEventArgs(ITurn turn)
            {
                Turn = turn;
            }

            public ITurn Turn { get; }
        }
    }
}