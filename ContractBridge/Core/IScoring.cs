namespace ContractBridge.Core
{
    public interface IScoring
    {
        IBoard Board { get; }

        IScoringStrategy ScoringStrategy { get; set; }

        IContract Contract { get; set; }

        int ForDeclarer();
    }
}