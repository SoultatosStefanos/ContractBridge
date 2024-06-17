namespace ContractBridge.Core
{
    public interface ISession
    {
        delegate void PhaseChangeHandler(ISession session, Phase phase);

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

        event PhaseChangeHandler PhaseChanged;
    }
}