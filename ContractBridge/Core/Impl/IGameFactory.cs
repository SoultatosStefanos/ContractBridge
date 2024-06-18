namespace ContractBridge.Core.Impl
{
    public interface IGameFactory
    {
        IGame Create(IBoard board);
    }
}