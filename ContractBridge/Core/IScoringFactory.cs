namespace ContractBridge.Core
{
    public interface IScoringFactory
    {
        IScoring NewScoring(IBoard board);
    }
}