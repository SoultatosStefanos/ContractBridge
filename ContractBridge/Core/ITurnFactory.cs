namespace ContractBridge.Core
{
    public interface ITurnFactory
    {
        ITurn NewTurn(Seat seat);
    }
}