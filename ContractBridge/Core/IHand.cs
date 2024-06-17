using System;

namespace ContractBridge.Core
{
    public interface IHand : ICardCollection
    {
        void Add(ICard card);

        void Remove(ICard card);

        void Clear();

        event EventHandler<AddEventArgs> Added;

        event EventHandler<RemoveEventArgs> Removed;

        event EventHandler<ClearEventArgs> Cleared;

        event EventHandler<EmptyEventArgs> Emptied;

        public sealed class AddEventArgs : EventArgs
        {
            public AddEventArgs(IHand hand, ICard card)
            {
                Hand = hand;
                Card = card;
            }

            public IHand Hand { get; }

            public ICard Card { get; }
        }

        public sealed class RemoveEventArgs : EventArgs
        {
            public RemoveEventArgs(IHand hand, ICard card)
            {
                Hand = hand;
                Card = card;
            }

            public IHand Hand { get; }

            public ICard Card { get; }
        }

        public sealed class ClearEventArgs : EventArgs
        {
            public ClearEventArgs(IHand hand)
            {
                Hand = hand;
            }

            public IHand Hand { get; }
        }

        public sealed class EmptyEventArgs : EventArgs
        {
            public EmptyEventArgs(IHand hand)
            {
                Hand = hand;
            }

            public IHand Hand { get; }
        }
    }
}