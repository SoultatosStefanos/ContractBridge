namespace ContractBridge.Core.Impl
{
    public class AuctionFactory : IAuctionFactory
    {
        private readonly IContractFactory _contractFactory;

        public AuctionFactory(IContractFactory contractFactory)
        {
            _contractFactory = contractFactory;
        }

        public IAuction Create(IBoard board)
        {
            return new Auction(board, _contractFactory);
        }
    }
}