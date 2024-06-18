namespace ContractBridge.Core.Impl
{
    public interface ICardFactory
    {
        ICard Create(Rank rank, Suit suit);
    }
}