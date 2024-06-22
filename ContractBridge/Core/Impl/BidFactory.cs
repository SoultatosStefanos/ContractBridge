namespace ContractBridge.Core.Impl
{
    public class BidFactory : IBidFactory
    {
        public IBid Create(Level level, Denomination denomination)
        {
            return new Bid(level, denomination);
        }
    }
}