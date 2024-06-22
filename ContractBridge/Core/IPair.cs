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
            public TrickEventArgs(ITrick trick)
            {
                Trick = trick;
            }

            public ITrick Trick { get; }
        }

        public sealed class ScoreEventArgs : EventArgs
        {
            public ScoreEventArgs(int score)
            {
                Score = score;
            }

            public int Score { get; }
        }
    }
}