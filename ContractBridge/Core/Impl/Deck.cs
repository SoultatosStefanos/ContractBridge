using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ContractBridge.Core.Impl
{
    internal static class RandomExtensions
    {
        internal static void Shuffle<T>(this Random rng, T[] array)
        {
            var n = array.Length;
            while (n > 1)
            {
                var k = rng.Next(n--);
                (array[n], array[k]) = (array[k], array[n]);
            }
        }
    }

    public class Deck : IDeck
    {
        private readonly ICard[] _cards;

        public Deck(ICardFactory cardFactory)
        {
            _cards = GenerateAllCardCombinations(cardFactory).ToArray();
            return;

            IEnumerable<ICard> GenerateAllCardCombinations(ICardFactory factory)
            {
                return from rank in Enum.GetValues(typeof(Rank)).Cast<Rank>()
                    from suit in Enum.GetValues(typeof(Suit)).Cast<Suit>()
                    select factory.Create(rank, suit);
            }
        }

        public IEnumerator<ICard> GetEnumerator()
        {
            return ((IEnumerable<ICard>)_cards).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _cards.Length;

        public ICard this[int index] => Get(index);

        public bool IsEmpty()
        {
            return _cards.Length == 0;
        }

        public bool Contains(ICard card)
        {
            return _cards.Contains(card);
        }

        public void Shuffle(Random rng)
        {
            rng.Shuffle(_cards);
            Shuffled?.Invoke(this, EventArgs.Empty);
        }

        public void Deal(IBoard board) // TODO
        {
            throw new NotImplementedException();
        }

        public event EventHandler<IDeck.DealEventArgs>? Dealt;

        public event EventHandler? Shuffled;

        private ICard Get(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException($"Card index {index} is out of range.");
            }

            return _cards[index];
        }
    }
}