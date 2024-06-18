using ContractBridge.Core;

namespace ContractBridge.Solver
{
    public interface IDoubleDummySolver
    {
        IDoubleDummySolution Analyze(IBoard board);
    }
}