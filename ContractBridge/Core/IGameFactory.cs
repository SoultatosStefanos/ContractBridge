namespace ContractBridge.Core
{
    public interface IGameFactory
    {
        IGame Create(IBoard board, ITrickFactory trickFactory);
    }
}