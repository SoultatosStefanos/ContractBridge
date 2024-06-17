namespace ContractBridge.Core
{
    public interface ISessionFactory
    {
        ISession NewSession();
    }
}