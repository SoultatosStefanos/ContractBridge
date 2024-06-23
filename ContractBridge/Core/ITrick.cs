using System;

namespace ContractBridge.Core
{
    public class TrickSizeNotFourException : Exception
    {
    }

    public interface ITrick : ICardCollection
    {
    }
}