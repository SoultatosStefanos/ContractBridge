using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ContractBridge.Core.Impl
{
    public class Trick : ITrick
    {
        private readonly ICard[] _cards;

        public Trick(ICard[] cards)
        {
            if (cards.Length != 4)
            {
                throw new TrickSizeNotFourException();
            }

            _cards = cards;
        }

        public IEnumerator<ICard> GetEnumerator()
        {
            return (IEnumerator<ICard>)_cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _cards.Length;

        public ICard this[int index] => _cards[index];

        public ICard this[Rank rank, Suit suit] => _cards.First(c => c.Rank == rank && c.Suit == suit);

        public bool IsEmpty()
        {
            return false;
        }

        public bool Contains(ICard card)
        {
            return _cards.Contains(card);
        }

        private bool Equals(Trick other)
        {
            return _cards.Equals(other._cards);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Trick)obj);
        }

        public override int GetHashCode()
        {
            return _cards.GetHashCode();
        }
    }
}