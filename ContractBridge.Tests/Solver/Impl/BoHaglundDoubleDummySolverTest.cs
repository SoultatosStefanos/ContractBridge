using System;
using ContractBridge.Core;
using ContractBridge.Core.Impl;
using ContractBridge.Solver.Impl;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace ContractBridge.Tests.Solver.Impl
{
    [TestFixture]
    [TestOf(typeof(BoHaglundDoubleDummySolver))]
    public class BoHaglundDoubleDummySolverTest
    {
        [SetUp]
        public void SetUp()
        {
            _solver = new BoHaglundDoubleDummySolver(new ContractFactory());

            _session = new Session(
                new Deck(new CardFactory()),
                new Board(new HandFactory()),
                new PairFactory(),
                new AuctionFactory(new ContractFactory()),
                new GameFactory(new TrickFactory()),
                new ScoringSystem()
            );
        }

        private Session _session;
        private BoHaglundDoubleDummySolver _solver;

        [Test]
        public void AnalyzeContractsDoesNotThrowOnAuction()
        {
            _session.Board.Dealer = Seat.North;
            _session.Deck.Shuffle(new Random());
            _session.Deck.Deal(_session.Board);

            Assert.DoesNotThrow(() => { _solver.AnalyzeContracts(_session); });
        }

        [Test]
        public void AnalyzeContractsDoesNotThrowOnPlay()
        {
            _session.Board.Dealer = Seat.North;
            _session.Deck.Shuffle(new Random());
            _session.Deck.Deal(_session.Board);

            Assert.That(_session.Auction, Is.Not.Null);

            _session.Auction.Call(new Bid(Level.Five, Denomination.Clubs), Seat.North);
            _session.Auction.Pass(Seat.East);
            _session.Auction.Pass(Seat.South);
            _session.Auction.Pass(Seat.West);

            Assert.DoesNotThrow(() => { _solver.AnalyzeContracts(_session); });
        }

        [Test]
        public void AnalyzePlaysDoesNotThrowOnPlay()
        {
            _session.Board.Dealer = Seat.North;
            _session.Deck.Shuffle(new Random());
            _session.Deck.Deal(_session.Board);

            Assert.That(_session.Auction, Is.Not.Null);

            _session.Auction.Call(new Bid(Level.Five, Denomination.Clubs), Seat.North);
            _session.Auction.Pass(Seat.East);
            _session.Auction.Pass(Seat.South);
            _session.Auction.Pass(Seat.West);

            Assert.That(_session.Auction.FinalContract, Is.Not.Null);

            Assert.DoesNotThrow(() => { _solver.AnalyzePlays(_session, _session.Auction.FinalContract); });
        }
    }
}