namespace ContractBridge.Core
{
    public interface IBidFactory
    {
        IBid Create(Level level, Denomination denomination);
    }
}