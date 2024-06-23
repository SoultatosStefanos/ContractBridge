namespace ContractBridge.Core.Impl
{
    public class AuctionFactory : IAuctionFactory
    {
        private readonly IContractFactory _contractFactory;
        private readonly ITurnPlayContext _turnPlayContext;

        public AuctionFactory(ITurnPlayContext turnPlayContext, IContractFactory contractFactory)
        {
            _turnPlayContext = turnPlayContext;
            _contractFactory = contractFactory;
        }

        public IAuction Create()
        {
            return new Auction(_turnPlayContext, _contractFactory);
        }
    }
}