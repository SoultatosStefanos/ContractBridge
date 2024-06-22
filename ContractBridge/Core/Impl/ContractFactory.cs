namespace ContractBridge.Core.Impl
{
    public class ContractFactory : IContractFactory
    {
        public IContract Create(Level level, Denomination denomination, Seat declarer, Risk? risk)
        {
            return new Contract(level, denomination, declarer, risk);
        }
    }
}