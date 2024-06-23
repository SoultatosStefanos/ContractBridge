namespace ContractBridge.Core
{
    public interface IScoringSystem
    {
        (int DeclarerScore, int DefenderScore) Score(IContract contract, bool vulnerable, int tricksMade);
    }
}