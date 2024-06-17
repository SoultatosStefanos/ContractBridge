namespace ContractBridge.Core
{
    public interface IBoardFactory
    {
        IBoard Create(IPairFactory pairFactory, IHandFactory handFactory);
    }
}