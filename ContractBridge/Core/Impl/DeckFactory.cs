namespace ContractBridge.Core.Impl
{
    public class DeckFactory : IDeckFactory
    {
        private readonly ICardFactory _cardFactory;

        public DeckFactory(ICardFactory cardFactory)
        {
            _cardFactory = cardFactory;
        }

        public IDeck Create()
        {
            return new Deck(_cardFactory);
        }
    }
}