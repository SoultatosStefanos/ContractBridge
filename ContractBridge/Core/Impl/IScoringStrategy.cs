namespace ContractBridge.Core.Impl
{
    public interface IScoringStrategy
    {
        int Score(IContract contract, bool vulnerable, int tricksWonCount);
    }
}