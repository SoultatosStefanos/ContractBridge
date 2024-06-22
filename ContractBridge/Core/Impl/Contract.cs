namespace ContractBridge.Core.Impl
{
    public class Contract : IContract
    {
        public Contract(Level level, Denomination denomination, Seat declarer, Risk? risk)
        {
            Level = level;
            Denomination = denomination;
            Declarer = declarer;
            Risk = risk;
        }

        public Level Level { get; }
        public Denomination Denomination { get; }
        public Seat Declarer { get; }
        public Risk? Risk { get; }

        private bool Equals(IContract other)
        {
            return Level == other.Level
                   && Denomination == other.Denomination
                   && Declarer == other.Declarer
                   && Risk == other.Risk;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Contract)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Level;
                hashCode = (hashCode * 397) ^ (int)Denomination;
                hashCode = (hashCode * 397) ^ (int)Declarer;
                hashCode = (hashCode * 397) ^ Risk.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                $"{nameof(Level)}: {Level}, {nameof(Denomination)}: {Denomination}, {nameof(Declarer)}: {Declarer}, {nameof(Risk)}: {Risk}";
        }
    }
}