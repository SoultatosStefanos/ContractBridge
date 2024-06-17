using System;

namespace ContractBridge.Core
{
    public interface ITurnManager
    {
        Seat? Lead { get; set; }

        ITurnFactory TurnFactory { get; }

        ITurn? NextTurn();

        void Restart();

        event EventHandler<LeadEventArgs> LeadSet;

        event EventHandler<TurnEventArgs> TurnChanged;

        event EventHandler<RestartEventArgs> Restarted;

        public sealed class LeadEventArgs : EventArgs
        {
            public LeadEventArgs(ITurnManager turnManager, Seat seat)
            {
                TurnManager = turnManager;
                Seat = seat;
            }

            public ITurnManager TurnManager { get; }

            public Seat Seat { get; }
        }

        public sealed class TurnEventArgs : EventArgs
        {
            public TurnEventArgs(ITurnManager turnManager, ITurn turn)
            {
                TurnManager = turnManager;
                Turn = turn;
            }

            public ITurnManager TurnManager { get; }

            public ITurn Turn { get; }
        }

        public sealed class RestartEventArgs : EventArgs
        {
            public RestartEventArgs(ITurnManager turnManager)
            {
                TurnManager = turnManager;
            }

            public ITurnManager TurnManager { get; }
        }
    }
}