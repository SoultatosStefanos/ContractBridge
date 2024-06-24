using System;

namespace ContractBridge.Core
{
    public class GamePlayException : Exception
    {
    }

    public class CardNotInHandException : GamePlayException
    {
    }

    public class GamePlayOutOfTurnException : GamePlayException
    {
    }

    public interface IGame
    {
        IBoard Board { get; }

        TrumpSuit TrumpSuit { get; set; }

        Seat? FirstLead { get; set; }

        Seat? Lead { get; }

        Seat? Turn { get; }

        bool CanFollow(ICard card, Seat seat);

        bool CanFollow(Seat seat);

        void Follow(ICard card, Seat seat);

        event EventHandler<LeadEventArgs> LeadChanged;

        event EventHandler<TurnEventArgs> TurnChanged;

        event EventHandler<FollowEventArgs> Followed;

        event EventHandler<TrickEventArgs> TrickWon;

        event EventHandler Done;

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
            public TurnEventArgs(Seat seat)
            {
                Seat = seat;
            }

            public Seat Seat { get; }
        }

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

    public static class GameExtensions
    {
        public static bool CanFollow(this IGame game, Rank rank, Suit suit, Seat seat)
        {
            return game.CanFollow(game.Board.Hand(seat)[rank, suit], seat);
        }

        public static void Follow(this IGame game, Rank rank, Suit suit, Seat seat)
        {
            game.Follow(game.Board.Hand(seat)[rank, suit], seat);
        }
    }
}