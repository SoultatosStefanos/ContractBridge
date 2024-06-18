namespace ContractBridge.Core.Impl
{
    public interface IScoringFactory
    {
        IScoring Create(IBoard board);
    }
}