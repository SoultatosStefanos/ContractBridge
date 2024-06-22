using System;
using System.Collections.Generic;

namespace ContractBridge.Core
{
    public interface IGame
    {
        IBoard Board { get; }

        IEnumerable<ICard> AllPlayedCards { get; }

        bool CanFollow(ICard card, ITurn turn);

        bool CanFollow(ITurn turn);

        bool CanPass(ITurn turn);

        void Follow(ICard card, ITurn turn);

        void Pass(ITurn turn);

        event EventHandler<FollowEventArgs> Followed;

        event EventHandler<PassEventArgs> Passed;

        event EventHandler<TrickEventArgs> TrickWon;

        event EventHandler Done;

        public sealed class FollowEventArgs : EventArgs
        {
            public FollowEventArgs(ICard card, ITurn turn)
            {
                Card = card;
                Turn = turn;
            }

            public ICard Card { get; }

            public ITurn Turn { get; }
        }

        public sealed class PassEventArgs : EventArgs
        {
            public PassEventArgs(ITurn turn)
            {
                Turn = turn;
            }

            public ITurn Turn { get; }
        }

        public sealed class TrickEventArgs : EventArgs
        {
            public TrickEventArgs(ITrick trick)
            {
                Trick = trick;
            }

            public ITrick Trick { get; }
        }
    }
}