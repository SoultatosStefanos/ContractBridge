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

        public Deck(ICardFactory cardFactory, IDeck.Partition partition = IDeck.Partition.ByRank)
        {
            _cards = GetGeneratorByPartition().ToArray();
            InitialPartition = partition;

            return;

            IEnumerable<ICard> GetGeneratorByPartition()
            {
                return partition == IDeck.Partition.ByRank
                    ? GenerateAllCardCombinationsByRank()
                    : GenerateAllCardCombinationsBySuit();
            }

            IEnumerable<ICard> GenerateAllCardCombinationsByRank()
            {
                return from rank in Enum.GetValues(typeof(Rank)).Cast<Rank>()
                    from suit in Enum.GetValues(typeof(Suit)).Cast<Suit>()
                    select cardFactory.Create(rank, suit);
            }

            IEnumerable<ICard> GenerateAllCardCombinationsBySuit()
            {
                return from suit in Enum.GetValues(typeof(Suit)).Cast<Suit>()
                    from rank in Enum.GetValues(typeof(Rank)).Cast<Rank>()
                    select cardFactory.Create(rank, suit);
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

        public ICard this[int index] => _cards[index];

        public ICard this[Rank rank, Suit suit] => _cards.First(c => c.Rank == rank && c.Suit == suit);

        public bool IsEmpty()
        {
            return _cards.Length == 0;
        }

        public bool Contains(ICard card)
        {
            return _cards.Contains(card);
        }

        public IDeck.Partition InitialPartition { get; }

        public void Shuffle(Random rng)
        {
            rng.Shuffle(_cards);

            RaiseShuffledEvent();
        }

        public void Deal(IBoard board)
        {
            if (board.Dealer is { } dealerValue)
            {
                ClearBoardHands();
                DealBoardHands();

                RaiseDealtEvent(board);
            }
            else
            {
                throw new DealerNotSetException();
            }

            return;

            void ClearBoardHands()
            {
                foreach (var hand in board.Hands)
                {
                    hand.Clear();
                }
            }

            void DealBoardHands()
            {
                _cards.Aggregate(dealerValue, (currentSeat, card) =>
                {
                    var nextSeat = currentSeat.NextSeat();
                    board.Hand(nextSeat).Add(card);
                    return nextSeat;
                });
            }
        }

        public event EventHandler<IDeck.DealEventArgs>? Dealt;

        public event EventHandler? Shuffled;

        private bool Equals(Deck other)
        {
            return _cards.Equals(other._cards);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Deck)obj);
        }

        public override int GetHashCode()
        {
            return _cards.GetHashCode();
        }

        private void RaiseDealtEvent(IBoard board)
        {
            Dealt?.Invoke(this, new IDeck.DealEventArgs(board));
        }

        private void RaiseShuffledEvent()
        {
            Shuffled?.Invoke(this, EventArgs.Empty);
        }
    }
}