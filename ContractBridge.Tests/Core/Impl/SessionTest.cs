using System;
using System.Linq;
using ContractBridge.Core;
using ContractBridge.Core.Impl;
using NUnit.Framework;

namespace ContractBridge.Tests.Core.Impl
{
    [TestFixture]
    [TestOf(typeof(Session))]
    public class SessionTest
    {
        [SetUp]
        public void SetUp()
        {
            _session = new Session(
                new Deck(new CardFactory(), IDeck.Partition.BySuit),
                new Board(new HandFactory()),
                new PairFactory(),
                new AuctionFactory(new ContractFactory()),
                new GameFactory(new TrickFactory()),
                new ScoringSystem()
            );
        }

        private Session _session;

        [Test]
        public void InitiallyIsAtSetupPhase()
        {
            Assert.That(_session.Phase, Is.EqualTo(Phase.Setup));
        }

        [Test]
        public void SetupPhase()
        {
            _session.Board.Dealer = Seat.North;
            _session.Deck.Shuffle(new Random());
            _session.Deck.Deal(_session.Board);

            Assert.Multiple(() =>
            {
                Assert.That(_session.Phase, Is.EqualTo(Phase.Auction));
                Assert.That(_session.Auction, Is.Not.Null);
            });
        }

        [Test]
        public void AuctionPhase()
        {
            _session.Board.Dealer = Seat.East;
            _session.Deck.Shuffle(new Random());
            _session.Deck.Deal(_session.Board);

            Assert.That(_session.Auction, Is.Not.Null);

            _session.Auction.Call(new Bid(Level.One, Denomination.NoTrumps), Seat.East);
            _session.Auction.Pass(Seat.South);
            _session.Auction.Pass(Seat.West);
            _session.Auction.Pass(Seat.North);

            Assert.Multiple(() =>
            {
                Assert.That(_session.Phase, Is.EqualTo(Phase.Play));
                Assert.That(_session.Game, Is.Not.Null);
                Assert.That(_session.Game.TrumpSuit, Is.EqualTo(TrumpSuit.NoTrump));
            });
        }

        [Test]
        public void PlayPhase()
        {
            _session.Board.Dealer = Seat.North;
            _session.Deck.Deal(_session.Board);

            Assert.That(_session.Auction, Is.Not.Null);

            _session.Auction.Call(new Bid(Level.One, Denomination.Hearts), Seat.North);
            _session.Auction.Pass(Seat.East);
            _session.Auction.Pass(Seat.South);
            _session.Auction.Pass(Seat.West);

            Assert.That(_session.Game, Is.Not.Null);

            _session.Game.Follow(Rank.Ace, Suit.Clubs, Seat.East);
            _session.Game.Follow(Rank.Three, Suit.Clubs, Seat.South);
            _session.Game.Follow(Rank.Four, Suit.Clubs, Seat.West);
            _session.Game.Follow(Rank.Five, Suit.Clubs, Seat.North);

            Assert.Multiple(() =>
            {
                Assert.That(_session.Phase, Is.EqualTo(Phase.Play));

                var trickWon = _session.Pair(Seat.East).AllTricksWon.First();

                Assert.That(trickWon.Contains(_session.Deck[Rank.Ace, Suit.Clubs]), Is.True);
                Assert.That(trickWon.Contains(_session.Deck[Rank.Three, Suit.Clubs]), Is.True);
                Assert.That(trickWon.Contains(_session.Deck[Rank.Four, Suit.Clubs]), Is.True);
                Assert.That(trickWon.Contains(_session.Deck[Rank.Five, Suit.Clubs]), Is.True);
            });
        }
    }
}