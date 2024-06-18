namespace ContractBridge.Core.Impl
{
    public class CardFactory : ICardFactory
    {
        public ICard Create(Rank rank, Suit suit)
        {
            return new Card(rank, suit);
        }
    }
}