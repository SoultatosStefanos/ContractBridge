namespace ContractBridge.Core
{
    public interface IDeckFactory
    {
        IDeck Create(ICardFactory cardFactory);
    }
}