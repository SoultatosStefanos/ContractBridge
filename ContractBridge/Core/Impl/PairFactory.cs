namespace ContractBridge.Core.Impl
{
    public class PairFactory : IPairFactory
    {
        public IPair Create(Partnership partnership)
        {
            return new Pair(partnership);
        }
    }
}