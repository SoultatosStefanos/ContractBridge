namespace ContractBridge.Core
{
    public interface IScoringFactory
    {
        IScoring Create(IBoard board);
    }
}