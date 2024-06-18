using System.Collections.Generic;

namespace ContractBridge.Core
{
    public interface ICardCollection : IEnumerable<ICard>
    {
        int Count { get; }

        ICard this[int index] { get; }

        ICard this[Rank rank, Suit suit] { get; }

        bool IsEmpty();

        bool Contains(ICard card);
    }
}