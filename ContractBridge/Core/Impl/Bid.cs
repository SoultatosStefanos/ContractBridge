namespace ContractBridge.Core.Impl
{
    public class Bid : IBid
    {
        public Bid(Level level, Denomination denomination)
        {
            Level = level;
            Denomination = denomination;
        }

        public Level Level { get; }

        public Denomination Denomination { get; }

        private bool Equals(IBid other)
        {
            return Level == other.Level && Denomination == other.Denomination;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Bid)obj);
        }
        
        public override string ToString()
        {
            return $"{nameof(Level)}: {Level}, {nameof(Denomination)}: {Denomination}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Level * 397) ^ (int)Denomination;
            }
        }
    }
}