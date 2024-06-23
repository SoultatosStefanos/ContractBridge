namespace ContractBridge.Core.Impl
{
    public class ScoringSystemFactory : IScoringSystemFactory
    {
        public IScoringSystem Create()
        {
            return new ScoringSystem();
        }
    }
}