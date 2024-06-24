namespace ContractBridge.Core.Impl
{
    public class TurnSequenceFactory : ITurnSequenceFactory
    {
        private readonly ITurnFactory _turnFactory;

        public TurnSequenceFactory(ITurnFactory turnFactory)
        {
            _turnFactory = turnFactory;
        }

        public ITurnSequence Create()
        {
            return new TurnSequence(_turnFactory);
        }
    }
}