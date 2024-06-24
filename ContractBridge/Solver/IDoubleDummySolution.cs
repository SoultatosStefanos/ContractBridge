using System.Collections.Generic;
using ContractBridge.Core;

namespace ContractBridge.Solver
{
    public interface IDoubleDummySolution
    {
        IContract? MakeableContract(Seat declarer, Denomination denomination);

        IEnumerable<IContract> MakeableContracts(Seat declarer);

        IEnumerable<ICard> OptimalPlays(IContract contract);
    }
}