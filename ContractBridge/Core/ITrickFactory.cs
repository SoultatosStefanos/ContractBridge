using System.Collections.Generic;

namespace ContractBridge.Core
{
    public interface ITrickFactory
    {
        ITrick NewTrick(IEnumerable<ICard> cards);
    }
}