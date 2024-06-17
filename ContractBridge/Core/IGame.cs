using System.Collections.Generic;

namespace ContractBridge.Core
{
    public interface IGame
    {
        delegate void DoneHandler(IGame game);

        delegate void FollowHandler(IGame game, ICard card, ITurn turn);

        delegate void PassHandler(IGame game, ITurn turn);

        delegate void TrickWonHandler(IGame game, ITrick trick);

        IBoard Board { get; }

        IEnumerable<ICard> AllPlayedCards { get; }

        ITrickFactory TrickFactory { get; }

        bool CanFollow(ICard card, ITurn turn);

        bool CanFollow(ITurn turn);

        bool CanPass(ITurn turn);

        void Follow(ICard card, ITurn turn);

        void Pass(ITurn turn);

        event FollowHandler Followed;

        event PassHandler Passed;

        event TrickWonHandler TrickWon;

        event DoneHandler Done;
    }
}