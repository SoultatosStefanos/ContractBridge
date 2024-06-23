namespace ContractBridge.Core.Impl
{
    public class GameFactory : IGameFactory
    {
        private readonly ITrickFactory _trickFactory;
        private readonly ITurnPlayContext _turnPlayContext;

        public GameFactory(ITurnPlayContext turnPlayContext, ITrickFactory trickFactory)
        {
            _turnPlayContext = turnPlayContext;
            _trickFactory = trickFactory;
        }

        public IGame Create(IBoard board)
        {
            return new Game(board, _turnPlayContext, _trickFactory);
        }
    }
}