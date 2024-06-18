using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ContractBridge.Core.Impl
{
    public class Hand : IHand
    {
        private readonly IList<ICard> _cards = new List<ICard>();

        public IEnumerator<ICard> GetEnumerator()
        {
            return _cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _cards.Count;

        public ICard this[int index] => _cards[index];

        public ICard this[Rank rank, Suit suit] => _cards.First(c => c.Rank == rank && c.Suit == suit);

        public bool IsEmpty()
        {
            return _cards.Count == 0;
        }

        public bool Contains(ICard card)
        {
            return _cards.Contains(card);
        }

        public void Add(ICard card)
        {
            if (Contains(card))
            {
                throw new CardAlreadyInHandException();
            }

            _cards.Add(card);

            RaiseAddedEvent(card);
        }

        public void Remove(ICard card)
        {
            if (!_cards.Remove(card))
            {
                return;
            }

            RaiseRemovedEvent(card);

            if (IsEmpty())
            {
                RaiseEmptiedEvent();
            }
        }

        public void Clear()
        {
            var wasEmpty = IsEmpty();

            _cards.Clear();

            RaiseClearedEvent();
            if (!wasEmpty)
            {
                RaiseEmptiedEvent();
            }
        }

        public event EventHandler<IHand.AddEventArgs>? Added;

        public event EventHandler<IHand.RemoveEventArgs>? Removed;

        public event EventHandler? Cleared;

        public event EventHandler? Emptied;

        private bool Equals(Hand other)
        {
            return _cards.Equals(other._cards);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Hand)obj);
        }

        public override int GetHashCode()
        {
            return _cards.GetHashCode();
        }

        private void RaiseClearedEvent()
        {
            Cleared?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseEmptiedEvent()
        {
            Emptied?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseRemovedEvent(ICard card)
        {
            Removed?.Invoke(this, new IHand.RemoveEventArgs(card));
        }

        private void RaiseAddedEvent(ICard card)
        {
            Added?.Invoke(this, new IHand.AddEventArgs(card));
        }
    }
}