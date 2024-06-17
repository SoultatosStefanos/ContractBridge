using System.Collections.Generic;

namespace ContractBridge.Core
{
    public interface IPair
    {
        delegate void TrickWonHandler(IPair pair, int score);

        Partnership Partnership { get; }

        IEnumerable<ITrick> AllTricksWon { get; }

        void WinTrick(ITrick trick);

        event TrickWonHandler TrickWon;
    }
}