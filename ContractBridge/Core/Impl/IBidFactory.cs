namespace ContractBridge.Core.Impl
{
    public interface IBidFactory
    {
        IBid Create(Level level, Denomination denomination);
    }
}