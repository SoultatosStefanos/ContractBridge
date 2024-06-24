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

        private Seat? _firstLead;

        private int _followCount;

        private Seat? _lead;

        private Seat? _nextTurn;

        public Game(IBoard board, ITrickFactory trickFactory)
        {
            Board = board;
            _trickFactory = trickFactory;
        }

        public IBoard Board { get; }

        public TrumpSuit TrumpSuit { get; set; }

        public Seat? FirstLead
        {
            get => _firstLead;
            set
            {
                _firstLead = value;

                if (FirstLead is not { } firstLead)
                {
                    return;
                }

                Turn = firstLead;
                Lead = firstLead;
            }
        }

        public Seat? Lead
        {
            get => _lead;
            private set
            {
                _lead = value;
                Debug.Assert(_lead != null, nameof(_lead) + " != null");
                RaiseLeadChangedEvent(_lead!.Value);
            }
        }

        public Seat? Turn
        {
            get => _nextTurn;
            private set
            {
                _nextTurn = value;
                Debug.Assert(_nextTurn != null, nameof(_nextTurn) + " != null");
                RaiseTurnChangedEvent(_nextTurn!.Value);
            }
        }

        public IEnumerable<ICard> PlayedCards => _playEntries.Select(entry => entry.Card);

        public bool CanFollow(ICard card, Seat seat)
        {
            var seatHand = Board.Hand(seat);

            if (!seatHand.Contains(card))
            {
                throw new CardNotInHandException();
            }

            if (Turn is not { } turn)
            {
                return false;
            }

            if (turn != seat)
            {
                return false;
            }

            if (LastPlayEntry() is not var (lastPlayCard, _))
            {
                return true;
            }

            return lastPlayCard.Suit == card.Suit || CantFollowSuit(lastPlayCard);

            bool CantFollowSuit(ICard lastCard)
            {
                return seatHand.All(handCard => handCard.Suit != lastCard.Suit);
            }
        }

        public bool CanFollow(Seat seat)
        {
            if (Turn is not { } turn)
            {
                return false;
            }

            return turn == seat;
        }

        public void Follow(ICard card, Seat seat)
        {
            var seatHand = Board.Hand(seat);

            if (!seatHand.Contains(card))
            {
                throw new CardNotInHandException();
            }

            if (Turn is not { } turn)
            {
                throw new GamePlayOutOfTurnException();
            }

            if (turn != seat)
            {
                throw new GamePlayOutOfTurnException();
            }

            _playEntries.Add(new PlayEntry(card, seat));
            seatHand.Remove(card);
            RaiseFollowedEvent(card, seat);

            if (SaveFollowAndCheckForAdvance())
            {
                var trickWinner = GetTrickWinner();
                var last4Cards = TakeLast4Cards();
                var trick = _trickFactory.Create(last4Cards);

                RaiseTrickWonEvent(trick, trickWinner);

                Turn = trickWinner;
                Lead = trickWinner;

                if (IsGameDone(seatHand))
                {
                    RaiseDoneEvent();
                }

                _followCount = 0;
            }
            else
            {
                Turn = turn.NextSeat();
            }
        }

        public event EventHandler<IGame.LeadEventArgs>? LeadChanged;

        public event EventHandler<IGame.TurnEventArgs>? TurnChanged;

        public event EventHandler<IGame.FollowEventArgs>? Followed;

        public event EventHandler<IGame.TrickEventArgs>? TrickWon;

        public event EventHandler? Done;

        private void RaiseLeadChangedEvent(Seat lead)
        {
            LeadChanged?.Invoke(this, new IGame.LeadEventArgs(lead));
        }

        private void RaiseTurnChangedEvent(Seat nextTurn)
        {
            TurnChanged?.Invoke(this, new IGame.TurnEventArgs(nextTurn));
        }

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