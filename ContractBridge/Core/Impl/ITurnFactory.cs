namespace ContractBridge.Core.Impl
{
    public interface ITurnFactory
    {
        ITurn Create(Seat seat);
    }
}