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

        event EventHandler<DoneEventArgs> Done;

        public sealed class FollowEventArgs : EventArgs
        {
            public FollowEventArgs(IGame game, ICard card, ITurn turn)
            {
                Game = game;
                Card = card;
                Turn = turn;
            }

            public IGame Game { get; }

            public ICard Card { get; }

            public ITurn Turn { get; }
        }

        public sealed class PassEventArgs : EventArgs
        {
            public PassEventArgs(IGame game, ITurn turn)
            {
                Game = game;
                Turn = turn;
            }

            public IGame Game { get; }

            public ITurn Turn { get; }
        }

        public sealed class TrickEventArgs : EventArgs
        {
            public TrickEventArgs(IGame game, ITrick trick)
            {
                Game = game;
                Trick = trick;
            }

            public IGame Game { get; }

            public ITrick Trick { get; }
        }

        public sealed class DoneEventArgs : EventArgs
        {
            public DoneEventArgs(IGame game)
            {
                Game = game;
            }

            public IGame Game { get; }
        }
    }
}