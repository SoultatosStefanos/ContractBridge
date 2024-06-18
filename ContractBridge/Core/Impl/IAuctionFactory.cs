namespace ContractBridge.Core
{
    public interface IAuctionFactory
    {
        IAuction Create(IBoard board);
    }
}