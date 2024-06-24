using System;

namespace ContractBridge.Core
{
    public class CardAlreadyInHandException : Exception
    {
    }

    public interface IHand : ICardCollection, IPbnSerializable
    {
        void Add(ICard card);

        void Remove(ICard card);

        void Clear();

        event EventHandler<AddEventArgs> Added;

        event EventHandler<RemoveEventArgs> Removed;

        event EventHandler Cleared;

        event EventHandler Emptied;

        public sealed class AddEventArgs : EventArgs
        {
            public AddEventArgs(ICard card)
            {
                Card = card;
            }

            public ICard Card { get; }
        }

        public sealed class RemoveEventArgs : EventArgs
        {
            public RemoveEventArgs(ICard card)
            {
                Card = card;
            }

            public ICard Card { get; }
        }
    }
}