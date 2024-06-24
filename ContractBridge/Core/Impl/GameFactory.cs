namespace ContractBridge.Core.Impl
{
    public class GameFactory : IGameFactory
    {
        private readonly ITrickFactory _trickFactory;

        public GameFactory(ITrickFactory trickFactory)
        {
            _trickFactory = trickFactory;
        }

        public IGame Create(IBoard board)
        {
            return new Game(board, _trickFactory);
        }
    }
}