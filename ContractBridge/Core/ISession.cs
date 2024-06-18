using System;
using System.Collections.Generic;

namespace ContractBridge.Core
{
    public interface ISession
    {
        Phase Phase { get; }

        IDeck Deck { get; }

        IBoard Board { get; }

        ITurnSequence TurnSequence { get; }

        IAuction? Auction { get; }

        IGame? Game { get; }

        IScoring? Scoring { get; }

        IEnumerable<IPair> Pairs { get; }

        IPair Pair(Seat seat);

        IPair OtherPair(Seat seat);

        IPair OtherPair(IPair pair);

        void Replay();

        event EventHandler<PhaseEventArgs> PhaseChanged;

        event EventHandler<ReplayEventArgs> Replayed;

        public sealed class PhaseEventArgs : EventArgs
        {
            public PhaseEventArgs(ISession session, Phase phase)
            {
                Session = session;
                Phase = phase;
            }

            public ISession Session { get; }

            public Phase Phase { get; }
        }

        public sealed class ReplayEventArgs : EventArgs
        {
            public ReplayEventArgs(ISession session)
            {
                Session = session;
            }

            public ISession Session { get; }
        }
    }
}