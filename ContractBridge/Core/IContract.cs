using System;

namespace ContractBridge.Core
{
    public interface IContract : IBid
    {
        Seat Declarer { get; }

        Risk? Risk { get; }
    }

    public static class ContractExtensions
    {
        public static Seat Dummy(this IContract contract)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}