using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly int _requiredFollowCountToAdvance = Enum.GetNames(typeof(Seat)).Length;

        private readonly ITrickFactory _trickFactory;

        private readonly ITurnPlayContext _turnPlayContext;

        private int _followCount;

        public Game(IBoard board, ITurnPlayContext turnPlayContext, ITrickFactory trickFactory)
        {
            Board = board;
            _turnPlayContext = turnPlayContext;
            _trickFactory = trickFactory;
        }

        public IBoard Board { get; }

        public TrumpSuit TrumpSuit { get; set; }

        public IEnumerable<ICard> AllPlayedCards => _playEntries.Select(entry => entry.Card);

        public bool CanFollow(ICard card, Seat seat)
        {
            var seatHand = Board.Hand(seat);

            if (!seatHand.Contains(card))
            {
                throw new CardNotInHandException();
            }

            return _turnPlayContext.CanPlayTurn(seat, () =>
            {
                if (LastPlayEntry() is not var (lastPlayCard, _))
                {
                    return true;
                }

                return lastPlayCard.Suit == card.Suit || CantFollowSuit(lastPlayCard);
            });

            bool CantFollowSuit(ICard lastPlayCard)
            {
                return seatHand.All(handCard => handCard.Suit != lastPlayCard.Suit);
            }
        }

        public bool CanFollow(Seat seat)
        {
            return _turnPlayContext.CanPlayTurn(seat, () => true);
        }

        public void Follow(ICard card, Seat seat)
        {
            var seatHand = Board.Hand(seat);

            if (!seatHand.Contains(card))
            {
                throw new CardNotInHandException();
            }

            _turnPlayContext.PlayTurn(seat, () =>
            {
                _playEntries.Add(new PlayEntry(card, seat));
                seatHand.Remove(card);
                RaiseFollowedEvent(card, seat);

                if (SaveFollowAndCheckForAdvance())
                {
                    var trickWinner = GetTrickWinner();
                    var last4Cards = TakeLast4Cards();
                    var trick = _trickFactory.Create(last4Cards);

                    RaiseTrickWonEvent(trick, trickWinner);

                    if (IsGameDone(seatHand))
                    {
                        RaiseDoneEvent();
                    }

                    _followCount = 0;
                }
            });
        }

        public event EventHandler<IGame.FollowEventArgs>? Followed;
        public event EventHandler<IGame.TrickEventArgs>? TrickWon;
        public event EventHandler? Done;

        private bool IsGameDone(IHand seatHand)
        {
            return seatHand.IsEmpty() && Board.OtherHands(seatHand).All(hand => hand.IsEmpty());
        }

        private Seat GetTrickWinner()
        {
            Debug.Assert(_playEntries.Count == 4);

            var last4 = _playEntries.Skip(_playEntries.Count - 4).ToList();

            var highestTrumpEntry = last4
                .Where(entry => (byte)entry.Card.Suit == (int)TrumpSuit)
                .OrderByDescending(entry => entry.Card.Rank)
                .FirstOrDefault();

            if (highestTrumpEntry != null)
            {
                return highestTrumpEntry.Seat;
            }

            var leadSuit = _playEntries.First().Card.Suit;

            var highestLeadEntry = last4
                .Where(entry => entry.Card.Suit == leadSuit) // Ignore discarded cards. 
                .OrderByDescending(entry => entry.Card.Rank)
                .First();

            return highestLeadEntry.Seat;
        }

        private bool SaveFollowAndCheckForAdvance()
        {
            return ++_followCount == _requiredFollowCountToAdvance;
        }

        private ICard[] TakeLast4Cards()
        {
            Debug.Assert(_playEntries.Count >= 4);

            var result = _playEntries
                .GetRange(_playEntries.Count - 4, 4)
                .Select(entry => entry.Card)
                .ToArray();

            _playEntries.RemoveRange(_playEntries.Count - 4, 4);

            return result;
        }

        private void RaiseFollowedEvent(ICard card, Seat seat)
        {
            Followed?.Invoke(this, new IGame.FollowEventArgs(card, seat));
        }

        private void RaiseTrickWonEvent(ITrick trick, Seat seat)
        {
            TrickWon?.Invoke(this, new IGame.TrickEventArgs(trick, seat));
        }

        private void RaiseDoneEvent()
        {
            Done?.Invoke(this, EventArgs.Empty);
        }

        private PlayEntry? LastPlayEntry()
        {
            return _playEntries.LastOrDefault();
        }
    }
}