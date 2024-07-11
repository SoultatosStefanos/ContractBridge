using System.Collections.Generic;
using ContractBridge.Core;

namespace ContractBridge.Solver
{
    public interface IDoubleDummyPlaysSolution
    {
        IEnumerable<ICard> OptimalPlays(Seat seat);
    }
}