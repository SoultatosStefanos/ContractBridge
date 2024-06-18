namespace ContractBridge.Core.Impl
{
    public interface IAuctionFactory
    {
        IAuction Create(IBoard board);
    }
}