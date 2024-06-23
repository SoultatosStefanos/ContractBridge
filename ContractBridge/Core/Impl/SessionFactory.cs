namespace ContractBridge.Core.Impl
{
    public class SessionFactory : ISessionFactory
    {
        private readonly IAuctionFactory _auctionFactory;

        private readonly IGameFactory _gameFactory;

        private readonly IPairFactory _pairFactory;

        public SessionFactory(IPairFactory pairFactory, IAuctionFactory auctionFactory, IGameFactory gameFactory)
        {
            _pairFactory = pairFactory;
            _auctionFactory = auctionFactory;
            _gameFactory = gameFactory;
        }

        public ISession Create(IDeck deck, IBoard board)
        {
            return new Session(deck, board, _pairFactory, _auctionFactory, _gameFactory);
        }
    }
}