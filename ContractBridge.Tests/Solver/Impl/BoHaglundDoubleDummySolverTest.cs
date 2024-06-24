using System;
using ContractBridge.Core;
using ContractBridge.Core.Impl;
using ContractBridge.Solver.Impl;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace ContractBridge.Tests.Solver.Impl
{
    [TestFixture]
    [TestOf(typeof(BoHaglundDoubleDummySolver))]
    public class BoHaglundDoubleDummySolverTest
    {
        private Session _session;
        private BoHaglundDoubleDummySolver _solver;

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

        [Test]
        public void DoesNotThrow()
        {
            _session.Board.Dealer = Seat.North;
            _session.Deck.Shuffle(new Random());
            _session.Deck.Deal(_session.Board);

            Assert.DoesNotThrow(() => { _solver.Analyze(_session); });
        }
    }
}