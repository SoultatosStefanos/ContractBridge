namespace ContractBridge.Core
{
    public interface IAuctionFactory
    {
        IAuction NewAuction(IBoard board, IContractFactory contractFactory);
    }
}