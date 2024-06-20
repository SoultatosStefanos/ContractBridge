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

        event EventHandler Restarted;

        public sealed class LeadEventArgs : EventArgs
        {
            public LeadEventArgs(Seat seat)
            {
                Seat = seat;
            }

            public Seat Seat { get; }
        }

        public sealed class TurnEventArgs : EventArgs
        {
            public TurnEventArgs(ITurn turn)
            {
                Turn = turn;
            }


            public ITurn Turn { get; }
        }

      
    }
}