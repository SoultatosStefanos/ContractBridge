namespace ContractBridge.Core.Impl
{
    public class TrickFactory : ITrickFactory
    {
        public ITrick Create(ICard[] cards)
        {
            return new Trick(cards);
        }
    }
}