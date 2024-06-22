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
    }
}