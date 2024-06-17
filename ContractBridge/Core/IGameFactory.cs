namespace ContractBridge.Core
{
    public interface IGameFactory
    {
        IGame NewGame(IBoard board, ITrickFactory trickFactory);
    }
}