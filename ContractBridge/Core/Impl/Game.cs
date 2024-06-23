using System;
using System.Collections.Generic;
using System.Linq;

namespace ContractBridge.Core.Impl
{
    internal class PlayEntry
    {
        public PlayEntry(ICard card, Seat seat)
        {
            Card = card;
            Seat = seat;
        }

        public ICard Card { get; }

        public Seat Seat { get; }

        public void Deconstruct(out ICard card, out Seat seat)
        {
            card = Card;
            seat = Seat;
        }
    }

    public class Game : IGame
    {
        private readonly List<PlayEntry> _playEntries = new();

        private readonly ITurnPlayContext _turnPlayContext;

        public Game(IBoard board, ITurnPlayContext turnPlayContext)
        {
            Board = board;
            _turnPlayContext = turnPlayContext;
        }

        public IBoard Board { get; }

        public TrumpSuit TrumpSuit { get; set; }

        public IEnumerable<ICard> AllPlayedCards => _playEntries.Select(entry => entry.Card);

        public bool CanFollow(ICard card, Seat seat)
        {
            if (!Board.Hand(seat).Contains(card))
            {
                throw new CardNotInHandException();
            }

            return _turnPlayContext.CanPlayTurn(seat, () =>
            {
                if (LastPlayEntry() is var (lastPlayCard, lastPlaySeat))
                {
                    if (lastPlayCard.Suit != card.Suit)
                    {
                        return Board.Hand(seat).All(handCard => handCard.Suit != lastPlayCard.Suit); // Follow suit.
                    }
                }

                // TODO Check for follow suit

                // TODO Check for discarding

                // TODO check for trumping

                return true;
            });
        }

        public bool CanFollow(Seat seat)
        {
            return Board.Hand(seat).Any(card => CanFollow(card, seat));
        }

        public void Follow(ICard card, Seat seat)
        {
            if (!Board.Hand(seat).Contains(card))
            {
                throw new CardNotInHandException();
            }

            _turnPlayContext.PlayTurn(seat, () =>
            {
                _playEntries.Add(new PlayEntry(card, seat));

                // TODO
            });
        }

        public event EventHandler<IGame.FollowEventArgs>? Followed;
        public event EventHandler<IGame.TrickEventArgs>? TrickWon;
        public event EventHandler? Done;

        private PlayEntry? LastPlayEntry()
        {
            return _playEntries.LastOrDefault();
        }
    }
}