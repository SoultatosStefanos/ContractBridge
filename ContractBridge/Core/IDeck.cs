namespace ContractBridge.Core
{
    public interface IDeck : ICardCollection
    {
        delegate void DealHandler(IDeck deck, IBoard board);

        delegate void ShuffleHandler(IDeck deck);

        ICardFactory CardFactory { get; }

        void Shuffle();

        void Deal(IBoard board);

        event ShuffleHandler Shuffled;

        event DealHandler Dealt;
    }
}