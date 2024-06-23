namespace ContractBridge.Core.Impl
{
    public interface ITrickFactory
    {
        ITrick Create(ICard[] cards);
    }
}