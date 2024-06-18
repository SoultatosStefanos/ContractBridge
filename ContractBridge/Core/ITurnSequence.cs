using System;

namespace ContractBridge.Core
{
    public interface ITurnSequence
    {
        Seat? Lead { get; set; }

        ITurn? NextTurn();

        void Restart();

        event EventHandler<LeadEventArgs> LeadSet;

        event EventHandler<TurnEventArgs> TurnChanged;

        event EventHandler<RestartEventArgs> Restarted;

        public sealed class LeadEventArgs : EventArgs
        {
            public LeadEventArgs(ITurnSequence turnSequence, Seat seat)
            {
                TurnSequence = turnSequence;
                Seat = seat;
            }

            public ITurnSequence TurnSequence { get; }

            public Seat Seat { get; }
        }

        public sealed class TurnEventArgs : EventArgs
        {
            public TurnEventArgs(ITurnSequence turnSequence, ITurn turn)
            {
                TurnSequence = turnSequence;
                Turn = turn;
            }

            public ITurnSequence TurnSequence { get; }

            public ITurn Turn { get; }
        }

        public sealed class RestartEventArgs : EventArgs
        {
            public RestartEventArgs(ITurnSequence turnSequence)
            {
                TurnSequence = turnSequence;
            }

            public ITurnSequence TurnSequence { get; }
        }
    }
}