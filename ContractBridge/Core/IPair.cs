using System;
using System.Collections.Generic;

namespace ContractBridge.Core
{
    public interface IPair
    {
        int Score { get; set; }

        Partnership Partnership { get; }

        IEnumerable<ITrick> AllTricksWon { get; }

        void WinTrick(ITrick trick);

        event EventHandler<TrickEventArgs> TrickWon;

        event EventHandler<ScoreEventArgs> Scored;

        public sealed class TrickEventArgs : EventArgs
        {
            public TrickEventArgs(IPair pair, ITrick trick)
            {
                Pair = pair;
                Trick = trick;
            }

            public IPair Pair { get; }

            public ITrick Trick { get; }
        }

        public sealed class ScoreEventArgs : EventArgs
        {
            public ScoreEventArgs(IPair pair, int score)
            {
                Pair = pair;
                Score = score;
            }

            public IPair Pair { get; }

            public int Score { get; }
        }
    }
}