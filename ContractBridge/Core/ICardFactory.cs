namespace ContractBridge.Core
{
    public interface ICardFactory
    {
        ICard NewCard(Rank rank, Suit suit);
    }
}