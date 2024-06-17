namespace ContractBridge.Core
{
    public interface IScoringStrategy
    {
        int Score(IContract contract, bool vulnerable, int tricksWonCount);
    }
}