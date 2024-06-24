using ContractBridge.Core.Impl;

namespace ContractBridge.Solver.Impl
{
    public class BoHaglundDoubleDummySolverFactory : IDoubleDummySolverFactory
    {
        private readonly IContractFactory _contractFactory;

        public BoHaglundDoubleDummySolverFactory(IContractFactory contractFactory)
        {
            _contractFactory = contractFactory;
        }

        public IDoubleDummySolver Create()
        {
            return new BoHaglundDoubleDummySolver(_contractFactory);
        }
    }
}