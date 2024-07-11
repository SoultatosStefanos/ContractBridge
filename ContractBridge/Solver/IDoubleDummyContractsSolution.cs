using System.Collections.Generic;
using ContractBridge.Core;

namespace ContractBridge.Solver
{
    public interface IDoubleDummyContractsSolution
    {
        IContract? MakeableContract(Seat declarer, Denomination denomination);

        IEnumerable<IContract> MakeableContracts(Seat declarer);
    }
}