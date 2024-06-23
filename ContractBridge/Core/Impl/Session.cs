using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ContractBridge.Core.Impl
{
    public class Session : ISession
    {
        private readonly IAuctionFactory _auctionFactory;
        private readonly IPair _eastWestPair;

        private readonly IGameFactory _gameFactory;

        private readonly IPair _northSouthPair;

        private readonly ITurnPlayContextFactory _turnPlayContextFactory;

        private readonly ITurnSequenceFactory _turnSequenceFactory;

        private Phase _phase = Phase.Setup;

        public Session(IDeck deck, IBoard board, IPairFactory pairFactory)
        {
            Deck = deck;
            Board = board;

            _northSouthPair = pairFactory.Create(Partnership.NorthSouth);
            _eastWestPair = pairFactory.Create(Partnership.EastWest);

            Deck.Dealt += OnDeckDealt;
        }

        public Phase Phase
        {
            get => _phase;
            private set
            {
                _phase = value;
                RaisePhaseChangedEvent(_phase);
            }
        }

        public IDeck Deck { get; }

        public IBoard Board { get; }

        public IAuction? Auction { get; private set; }

        public IGame? Game { get; private set; }

        public IEnumerable<IPair> Pairs => new[] { _northSouthPair, _eastWestPair };

        public IPair Pair(Seat seat)
        {
            return seat.Partnership() switch
            {
                Partnership.EastWest => _eastWestPair,
                Partnership.NorthSouth => _northSouthPair,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public IPair OtherPair(Seat seat)
        {
            return seat.Partnership() switch
            {
                Partnership.EastWest => _northSouthPair,
                Partnership.NorthSouth => _eastWestPair,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public IPair OtherPair(IPair pair)
        {
            if (pair != _northSouthPair && pair != _eastWestPair)
            {
                throw new UnknownPairException();
            }

            return pair.Partnership switch
            {
                Partnership.EastWest => _eastWestPair,
                Partnership.NorthSouth => _northSouthPair,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Replay()
        {
            // TODO
        }

        public event EventHandler<ISession.PhaseEventArgs>? PhaseChanged;
        public event EventHandler? Replayed;

        private void RaisePhaseChangedEvent(Phase phase)
        {
            PhaseChanged?.Invoke(this, new ISession.PhaseEventArgs(phase));
        }

        private void OnDeckDealt(object sender, EventArgs args)
        {
            Phase = Phase.Auction;

            Auction = _auctionFactory.Create();
            Auction.TurnPlayContext.TurnSequence.Lead = Board.Dealer;
            Auction.FinalContractMade += OnFinalContractMade;
            Auction.PassedOut += OnAuctionPassedOut;
        }

        private void OnFinalContractMade(object sender, EventArgs args)
        {
            Phase = Phase.Play;

            Debug.Assert(Auction != null);
            Debug.Assert(Auction!.FinalContract != null);

            Game = _gameFactory.Create(Board);
            Game.TurnPlayContext.TurnSequence.Lead = Auction!.FinalContract!.Declarer.NextSeat();
            Game.TrickWon += OnTrickWon;
            Game.Done += OnGameDone;
        }

        private void OnTrickWon(object sender, EventArgs args)
        {
            var trick = ((IGame.TrickEventArgs)args).Trick;
            var trickWinner = ((IGame.TrickEventArgs)args).Seat;

            Pair(trickWinner).WinTrick(trick);
        }

        private void OnGameDone(object sender, EventArgs args)
        {
            // TODO
        }

        private void OnAuctionPassedOut(object sender, EventArgs args)
        {
            Replay();
        }
    }
}