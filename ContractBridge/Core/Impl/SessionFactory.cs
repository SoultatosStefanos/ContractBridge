namespace ContractBridge.Core.Impl
{
    public class SessionFactory : ISessionFactory
    {
        private readonly IAuctionFactory _auctionFactory;

        private readonly IGameFactory _gameFactory;

        private readonly IPairFactory _pairFactory;

        private readonly IScoringSystem _scoringSystem;

        public SessionFactory(
            IPairFactory pairFactory, 
            IAuctionFactory auctionFactory, 
            IGameFactory gameFactory,  
            IScoringSystem scoringSystem
         )
        {
            _pairFactory = pairFactory;
            _auctionFactory = auctionFactory;
            _gameFactory = gameFactory;
            _scoringSystem = scoringSystem;
        }

        public ISession Create(IDeck deck, IBoard board)
        {
            return new Session(deck, board, _pairFactory, _auctionFactory, _gameFactory, _scoringSystem);
        }
    }
}