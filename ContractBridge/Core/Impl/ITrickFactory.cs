using System.Collections.Generic;

namespace ContractBridge.Core
{
    public interface ITrickFactory
    {
        ITrick Create(IEnumerable<ICard> cards);
    }
}