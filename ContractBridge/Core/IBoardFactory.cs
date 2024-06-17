namespace ContractBridge.Core
{
    public interface IBoardFactory
    {
        IBoard NewBoard(IPairFactory pairFactory, IHandFactory handFactory);
    }
}