namespace ContractBridge.Core
{
    public interface IScoring
    {
        IBoard Board { get; }

        IContract? Contract { get; set; }

        int DeclarerScore();
    }
}