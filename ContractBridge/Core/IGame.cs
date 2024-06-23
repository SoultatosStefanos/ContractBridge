using System;
using System.Collections.Generic;

namespace ContractBridge.Core
{
    public class CardNotInHandException : Exception
    {
    }

    public interface IGame
    {
        IBoard Board { get; }

        TrumpSuit TrumpSuit { get; set; }

        IEnumerable<ICard> AllPlayedCards { get; }

        bool CanFollow(ICard card, Seat seat);

        bool CanFollow(Seat seat);

        void Follow(ICard card, Seat seat);

        event EventHandler<FollowEventArgs> Followed;

        event EventHandler<TrickEventArgs> TrickWon;

        event EventHandler Done;

        public sealed class FollowEventArgs : EventArgs
        {
            public FollowEventArgs(ICard card, Seat seat)
            {
                Card = card;
                Seat = seat;
            }

            public ICard Card { get; }

            public Seat Seat { get; }
        }

        public sealed class TrickEventArgs : EventArgs
        {
            public TrickEventArgs(ITrick trick, Seat seat)
            {
                Trick = trick;
                Seat = seat;
            }

            public ITrick Trick { get; }

            public Seat Seat { get; }
        }
    }
}