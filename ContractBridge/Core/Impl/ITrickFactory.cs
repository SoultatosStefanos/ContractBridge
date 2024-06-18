using System.Collections.Generic;

namespace ContractBridge.Core.Impl
{
    public interface ITrickFactory
    {
        ITrick Create(IEnumerable<ICard> cards);
    }
}