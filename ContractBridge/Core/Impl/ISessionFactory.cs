namespace ContractBridge.Core
{
    public interface ISessionFactory
    {
        ISession Create(IDeck deck, IBoard board, ITurnManager turnManager);
    }
}