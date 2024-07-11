using System;
using ContractBridge.Core;

namespace ContractBridge.Solver
{
    public class DoubleDummySolverException : Exception
    {
        public DoubleDummySolverException()
        {
        }

        public DoubleDummySolverException(string message) : base(message)
        {
        }
    }

    public class InvalidPhaseForSolving : DoubleDummySolverException
    {
    }

    public interface IDoubleDummySolver
    {
        IDoubleDummyContractsSolution AnalyzeContracts(ISession session);

        IDoubleDummyPlaysSolution AnalyzePlays(ISession session, IContract contract);
    }
}