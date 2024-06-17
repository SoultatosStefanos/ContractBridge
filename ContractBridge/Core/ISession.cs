using System;

namespace ContractBridge.Core
{
    public interface ISession
    {
        Phase Phase { get; }

        IDeck Deck { get; }

        IBoard Board { get; }

        ITurnManager TurnManager { get; }

        IAuctionFactory AuctionFactory { get; }

        IGameFactory GameFactory { get; }

        IScoringFactory ScoringFactory { get; }

        IAuction? Auction { get; }

        IGame? Game { get; }

        IScoring? Scoring { get; }

        event EventHandler<PhaseEventArgs> PhaseChanged;

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
    }
}