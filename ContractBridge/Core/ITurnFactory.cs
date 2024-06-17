namespace ContractBridge.Core
{
    public interface ITurnFactory
    {
        ITurn Create(Seat seat);
    }
}