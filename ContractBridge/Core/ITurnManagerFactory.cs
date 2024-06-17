namespace ContractBridge.Core
{
    public interface ITurnManagerFactory
    {
        ITurnManager Create(ITurnFactory turnFactory);
    }
}