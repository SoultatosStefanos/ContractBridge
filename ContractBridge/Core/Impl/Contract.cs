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
    }
}