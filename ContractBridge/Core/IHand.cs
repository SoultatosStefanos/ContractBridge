namespace ContractBridge.Core
{
    public interface IHand : ICardCollection
    {
        public delegate void AddHandler(IHand hand, ICard card);

        public delegate void ClearHandler(IHand hand, ICard card);

        public delegate void EmptyHandler(IHand hand, ICard card);

        public delegate void RemoveHandler(IHand hand, ICard card);

        void Add(ICard card);

        void Remove(ICard card);

        void Clear();

        event AddHandler Added;

        event RemoveHandler Removed;

        event ClearHandler Cleared;

        event EmptyHandler Emptied;
    }
}