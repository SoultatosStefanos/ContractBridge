using System;
using System.Collections.Generic;

namespace ContractBridge.Core
{
    public class UnknownPairException : Exception
    {
    }

    public interface ISession
    {
        Phase Phase { get; }

        IDeck Deck { get; }

        IBoard Board { get; }

        IAuction? Auction { get; }

        IGame? Game { get; }

        IEnumerable<IPair> Pairs { get; }

        IPair Pair(Seat seat);

        IPair OtherPair(Seat seat);

        IPair OtherPair(IPair pair);

        void Reset();

        event EventHandler<PhaseEventArgs> PhaseChanged;

        public sealed class PhaseEventArgs : EventArgs
        {
            public PhaseEventArgs(Phase phase)
            {
                Phase = phase;
            }

            public Phase Phase { get; }
        }
    }
}