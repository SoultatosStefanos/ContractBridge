using System.Collections.Generic;
using ContractBridge.Core;

namespace ContractBridge.Solver
{
    public interface IDoubleDummySolution
    {
        IContract MakeableContract(Seat declaror, Denomination denomination);

        IEnumerable<IContract> MakeableContracts(Seat declaror);

        IEnumerable<ICard> OptimalPlays(IContract contract);
    }
}