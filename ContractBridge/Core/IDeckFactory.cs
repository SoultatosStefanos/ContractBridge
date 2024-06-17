namespace ContractBridge.Core
{
    public interface IDeckFactory
    {
        IDeck NewDeck(ICardFactory cardFactory);
    }
}