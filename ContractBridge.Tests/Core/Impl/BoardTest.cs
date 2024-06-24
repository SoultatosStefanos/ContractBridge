using System.Linq;
using ContractBridge.Core;
using ContractBridge.Core.Impl;
using NUnit.Framework;

namespace ContractBridge.Tests.Core.Impl
{
    [TestFixture]
    [TestOf(typeof(Board))]
    public class BoardTest
    {
        [SetUp]
        public void SetUp()
        {
            _board = new Board(new HandFactory());
        }

        private Board _board;

        [Test]
        public void InitiallyDealerIsNull()
        {
            Assert.That(_board.Dealer, Is.Null);
        }

        [Test]
        public void InitiallyVulnerabilityIsNull()
        {
            Assert.That(_board.Vulnerability, Is.Null);
        }

        [Test]
        public void SetDealerDoesNotRaiseDealerSetEventWithNullDealer()
        {
            var eventRaised = false;

            _board.DealerSet += (sender, args) => eventRaised = true;

            _board.Dealer = null;

            Assert.That(eventRaised, Is.False);
        }

        [Test]
        public void SetDealerRaisesDealerSetEventWithNonNullDealer()
        {
            var eventRaised = false;

            _board.DealerSet += (sender, args) => eventRaised = true;

            _board.Dealer = Seat.East;

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void SetVulnerabilityDoesNotRaiseVulnerabilitySetEventWithNullVulnerability()
        {
            var eventRaised = false;

            _board.VulnerabilitySet += (sender, args) => eventRaised = true;

            _board.Vulnerability = null;

            Assert.That(eventRaised, Is.False);
        }

        [Test]
        public void SetVulnerabilityRaisesVulnerabilitySetEventWithNonNullVulnerability()
        {
            var eventRaised = false;

            _board.VulnerabilitySet += (sender, args) => eventRaised = true;

            _board.Vulnerability = Vulnerability.EastWest;

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void HasOneHandForEachSeat()
        {
            var hands = _board.Hands;

            Assert.That(hands.Count(), Is.EqualTo(4));
        }

        [Test]
        public void DifferentHandBySeat()
        {
            var northHand = _board.Hand(Seat.North);
            var southHand = _board.Hand(Seat.South);
            var eastHand = _board.Hand(Seat.East);
            var westHand = _board.Hand(Seat.West);

            Assert.Multiple(() =>
            {
                Assert.That(ReferenceEquals(northHand, southHand), Is.False);
                Assert.That(ReferenceEquals(southHand, eastHand), Is.False);
                Assert.That(ReferenceEquals(eastHand, westHand), Is.False);
                Assert.That(ReferenceEquals(northHand, eastHand), Is.False);
                Assert.That(ReferenceEquals(southHand, westHand), Is.False);
            });
        }

        [Test]
        public void OtherHandsOfSeat()
        {
            var otherHands = _board.OtherHands(Seat.West);

            Assert.Multiple(() =>
            {
                var enumerable = otherHands as IHand[] ?? otherHands.ToArray();

                Assert.That(enumerable.Count(), Is.EqualTo(3));
                Assert.That(enumerable.Any(h => ReferenceEquals(h, _board.Hand(Seat.West))), Is.False);
            });
        }

        [Test]
        public void OtherHandsOfHand()
        {
            var hand = _board.Hand(Seat.East);
            var otherHands = _board.OtherHands(hand);

            Assert.Multiple(() =>
            {
                var enumerable = otherHands as IHand[] ?? otherHands.ToArray();

                Assert.That(enumerable.Count(), Is.EqualTo(3));
                Assert.That(enumerable.Any(h => ReferenceEquals(h, hand)), Is.False);
            });
        }

        [Test]
        public void InitiallyNoSeatIsVulnerable()
        {
            Assert.That(_board.IsVulnerable(Seat.East), Is.False);
            Assert.That(_board.IsVulnerable(Seat.West), Is.False);
            Assert.That(_board.IsVulnerable(Seat.North), Is.False);
            Assert.That(_board.IsVulnerable(Seat.South), Is.False);
        }

        [Test]
        public void AllSeatsAreVulnerableWithAllVulnerability()
        {
            _board.Vulnerability = Vulnerability.All;

            Assert.That(_board.IsVulnerable(Seat.East), Is.True);
            Assert.That(_board.IsVulnerable(Seat.West), Is.True);
            Assert.That(_board.IsVulnerable(Seat.North), Is.True);
            Assert.That(_board.IsVulnerable(Seat.South), Is.True);
        }

        [Test]
        public void NorthSouthSeatsAreVulnerableWithNorthSouthVulnerability()
        {
            _board.Vulnerability = Vulnerability.NorthSouth;

            Assert.That(_board.IsVulnerable(Seat.East), Is.False);
            Assert.That(_board.IsVulnerable(Seat.West), Is.False);
            Assert.That(_board.IsVulnerable(Seat.North), Is.True);
            Assert.That(_board.IsVulnerable(Seat.South), Is.True);
        }

        [Test]
        public void EastWestSeatsAreVulnerableWithEastWestVulnerability()
        {
            _board.Vulnerability = Vulnerability.EastWest;

            Assert.That(_board.IsVulnerable(Seat.East), Is.True);
            Assert.That(_board.IsVulnerable(Seat.West), Is.True);
            Assert.That(_board.IsVulnerable(Seat.North), Is.False);
            Assert.That(_board.IsVulnerable(Seat.South), Is.False);
        }

        [Test]
        public void ToPbn()
        {
            _board.Dealer = Seat.West;

            var hand1 = _board.Hand(Seat.West);

            hand1.Add(new Card(Rank.King, Suit.Spades));
            hand1.Add(new Card(Rank.Queen, Suit.Spades));
            hand1.Add(new Card(Rank.Ten, Suit.Spades));
            hand1.Add(new Card(Rank.Two, Suit.Spades));
            hand1.Add(new Card(Rank.Ace, Suit.Hearts));
            hand1.Add(new Card(Rank.Ten, Suit.Hearts));
            hand1.Add(new Card(Rank.Jack, Suit.Diamonds));
            hand1.Add(new Card(Rank.Six, Suit.Diamonds));
            hand1.Add(new Card(Rank.Five, Suit.Diamonds));
            hand1.Add(new Card(Rank.Four, Suit.Diamonds));
            hand1.Add(new Card(Rank.Two, Suit.Diamonds));
            hand1.Add(new Card(Rank.Eight, Suit.Clubs));
            hand1.Add(new Card(Rank.Five, Suit.Clubs));

            Assert.That(_board.ToPbn(), Is.EqualTo("W:KQT2.AT.J6542.85 - - -"));
        }
    }
}