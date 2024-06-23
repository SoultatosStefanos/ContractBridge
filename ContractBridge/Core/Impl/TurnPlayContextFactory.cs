namespace ContractBridge.Core.Impl
{
    public class TurnPlayContextFactory : ITurnPlayContextFactory
    {
        private readonly ITurnSequence _turnSequence;

        public TurnPlayContextFactory(ITurnSequence turnSequence)
        {
            _turnSequence = turnSequence;
        }

        public ITurnPlayContext Create()
        {
            return new TurnPlayContext(_turnSequence);
        }
    }
}