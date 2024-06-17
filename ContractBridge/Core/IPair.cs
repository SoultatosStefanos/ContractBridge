using System;
using System.Collections.Generic;

namespace ContractBridge.Core
{
    public interface IPair
    {
        Partnership Partnership { get; }

        IEnumerable<ITrick> AllTricksWon { get; }

        void WinTrick(ITrick trick);

        event EventHandler<TrickEventArgs> TrickWon;

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
    }
}