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