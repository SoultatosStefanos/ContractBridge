using System;
using System.Collections.Generic;

namespace ContractBridge.Core.Impl
{
    public class Pair : IPair
    {
        private readonly List<ITrick> _tricksWon = new();
        private int _score;

        public Pair(Partnership partnership)
        {
            Partnership = partnership;
        }

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                RaiseScoredEvent(_score);
            }
        }

        public Partnership Partnership { get; }

        public IEnumerable<ITrick> AllTricksWon => _tricksWon;

        public void WinTrick(ITrick trick)
        {
            _tricksWon.Add(trick);
            RaiseTrickWonEvent(trick);
        }

        public event EventHandler<IPair.TrickEventArgs>? TrickWon;

        public event EventHandler<IPair.ScoreEventArgs>? Scored;

        private void RaiseScoredEvent(int score)
        {
            Scored?.Invoke(this, new IPair.ScoreEventArgs(score));
        }

        private void RaiseTrickWonEvent(ITrick trick)
        {
            TrickWon?.Invoke(this, new IPair.TrickEventArgs(trick));
        }
    }
}