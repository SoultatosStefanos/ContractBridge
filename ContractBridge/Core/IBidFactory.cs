namespace ContractBridge.Core
{
    public interface IBidFactory
    {
        IBid NewBid(Level level, Denomination denomination);
    }
}