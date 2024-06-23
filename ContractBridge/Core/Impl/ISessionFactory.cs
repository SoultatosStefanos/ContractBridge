namespace ContractBridge.Core.Impl
{
    public interface ISessionFactory
    {
        ISession Create(IDeck deck, IBoard board);
    }
}