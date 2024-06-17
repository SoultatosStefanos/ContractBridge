namespace ContractBridge.Core
{
    public interface IBidFactory
    {
        IBid NewBid(byte level, Denomination denomination);
    }
}