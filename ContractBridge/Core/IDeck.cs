using System;

namespace ContractBridge.Core
{
    public class DealerNotSetException : Exception
    {
    }

    public interface IDeck : ICardCollection
    {
        void Shuffle(Random rng);

        void Deal(IBoard board);

        event EventHandler Shuffled;

        event EventHandler<DealEventArgs> Dealt;

        public sealed class DealEventArgs : EventArgs
        {
            public DealEventArgs(IBoard board)
            {
                Board = board;
            }

            public IBoard Board { get; }
        }
    }
}