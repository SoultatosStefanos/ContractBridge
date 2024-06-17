using System;

namespace ContractBridge.Core
{
    public interface IDeck : ICardCollection
    {
        ICardFactory CardFactory { get; }

        void Shuffle();

        void Deal(IBoard board);

        event EventHandler<ShuffleEventArgs> Shuffled;

        event EventHandler<DealEventArgs> Dealt;

        public sealed class DealEventArgs : EventArgs
        {
            public DealEventArgs(IDeck deck, IBoard board)
            {
                Deck = deck;
                Board = board;
            }

            public IDeck Deck { get; }

            public IBoard Board { get; }
        }

        public sealed class ShuffleEventArgs : EventArgs
        {
            public ShuffleEventArgs(IDeck deck)
            {
                Deck = deck;
            }

            public IDeck Deck { get; }
        }
    }
}