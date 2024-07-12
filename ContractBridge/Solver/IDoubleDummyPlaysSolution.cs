using System.Collections.Generic;
using ContractBridge.Core;

namespace ContractBridge.Solver
{
    public enum Priority : byte
    {
        Low = 0,
        Medium,
        High
    }

    public interface IDoubleDummyPlaysSolution
    {
        IEnumerable<(ICard, Priority)> OptimalPlays(Seat seat);
    }
}