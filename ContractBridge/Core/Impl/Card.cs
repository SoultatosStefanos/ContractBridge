using System;
using System.Text;

namespace ContractBridge.Core.Impl
{
    public class Card : ICard
    {
        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public Rank Rank { get; }
        public Suit Suit { get; }

        public string ToPbn()
        {
            var s = new StringBuilder(3);

            switch (Suit)
            {
                case Suit.Spades:
                    s.Append('S');
                    break;

                case Suit.Hearts:
                    s.Append('H');
                    break;

                case Suit.Diamonds:
                    s.Append('D');
                    break;

                case Suit.Clubs:
                    s.Append('C');
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (Rank)
            {
                case Rank.Two:
                    s.Append('2');

                    break;
                case Rank.Three:
                    s.Append('3');

                    break;
                case Rank.Four:
                    s.Append('4');
                    break;

                case Rank.Five:
                    s.Append('5');
                    break;

                case Rank.Six:
                    s.Append('6');
                    break;

                case Rank.Seven:
                    s.Append('7');
                    break;

                case Rank.Eight:
                    s.Append('8');
                    break;

                case Rank.Nine:
                    s.Append('9');
                    break;

                case Rank.Ten:
                    s.Append('T');
                    break;

                case Rank.Jack:
                    s.Append('J');
                    break;

                case Rank.Queen:
                    s.Append('Q');
                    break;

                case Rank.King:
                    s.Append('K');
                    break;

                case Rank.Ace:
                    s.Append('A');
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return s.ToString();
        }

        private bool Equals(ICard other)
        {
            return Rank == other.Rank && Suit == other.Suit;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Card)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Rank * 397) ^ (int)Suit;
            }
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }
}