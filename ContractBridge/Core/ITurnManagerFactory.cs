namespace ContractBridge.Core
{
    public interface ITurnManagerFactory
    {
        ITurnManager NewTurnManager(ITurnFactory turnFactory);
    }
}