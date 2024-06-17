namespace ContractBridge.Core
{
    public interface ICardFactory
    {
        ICard Create(Rank rank, Suit suit);
    }
}