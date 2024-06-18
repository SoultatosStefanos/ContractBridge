namespace ContractBridge.Core.Impl
{
    public class BoardFactory : IBoardFactory
    {
        private readonly IHandFactory _handFactory;

        public BoardFactory(IHandFactory handFactory)
        {
            _handFactory = handFactory;
        }

        public IBoard Create()
        {
            return new Board(_handFactory);
        }
    }
}