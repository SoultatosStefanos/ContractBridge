using System;
using System.Collections.Generic;
using System.Linq;
using ContractBridge.Core;
using ContractBridge.Core.Impl;
using NUnit.Framework;

namespace ContractBridge.Tests.Core.Impl
{
    [TestFixture]
    [TestOf(typeof(Deck))]
    public class DeckTest
    {
        [SetUp]
        public void SetUp()
        {
            _deck = new Deck(new CardFactory());
            _board = new Board(new HandFactory());
        }

        private Deck _deck;

        private IBoard _board;

        [Test]
        public void CountIs52()
        {
            Assert.That(_deck.Count, Is.EqualTo(52));
        }

        [Test]
        public void ContainsNoDuplicates()
        {
            Assert.That(ContainsDuplicates(), Is.False);
        }

        [Test]
        public void OutOfBoundsIndexingOver()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var unused = _deck[52];
            });
        }

        [Test]
        public void OutOfBoundsIndexingUnder()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var unused = _deck[-1];
            });
        }

        [Test]
        public void AccessingFirst4Cards()
        {
            var card0 = _deck[0];
            var card1 = _deck[1];
            var card2 = _deck[2];
            var card3 = _deck[3];

            Assert.Multiple(() =>
            {
                Assert.That(new Card((Rank)2, Suit.Clubs), Is.EqualTo(card0));
                Assert.That(new Card((Rank)2, Suit.Diamonds), Is.EqualTo(card1));
                Assert.That(new Card(Rank.Two, Suit.Hearts), Is.EqualTo(card2));
                Assert.That(new Card(Rank.Two, Suit.Spades), Is.EqualTo(card3));
            });
        }

        [Test]
        public void AccessingBySuitAndRank()
        {
            var card = _deck[Rank.Ace, Suit.Clubs];

            Assert.That(card.Rank, Is.EqualTo(Rank.Ace));
            Assert.That(card.Suit, Is.EqualTo(Suit.Clubs));
        }

        [Test]
        public void ShuffleAndThenAccess()
        {
            _deck.Shuffle(new Random());

            Assert.DoesNotThrow(() =>
            {
                var unused = _deck[0];
            });
        }

        [Test]
        public void ShuffleAndThenContainsNoDuplicates()
        {
            _deck.Shuffle(new Random());

            Assert.That(ContainsDuplicates(), Is.False);
        }

        [Test]
        public void ShuffleRaisesShuffledEvent()
        {
            var eventRaised = false;

            _deck.Shuffled += (sender, args) => eventRaised = true;

            _deck.Shuffle(new Random());

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void DealThrowsDealerNotSetExceptionWithoutDealer()
        {
            Assert.Throws<DealerNotSetException>(() => { _deck.Deal(_board); });
        }

        [Test]
        public void DealWithEmptyBoard()
        {
            _board.Dealer = Seat.East;

            _deck.Deal(_board);

            Assert.Multiple(() =>
            {
                Assert.That(_board.Hand(Seat.West).Count, Is.EqualTo(13));
                Assert.That(_board.Hand(Seat.East).Count, Is.EqualTo(13));
                Assert.That(_board.Hand(Seat.North).Count, Is.EqualTo(13));
                Assert.That(_board.Hand(Seat.South).Count, Is.EqualTo(13));

                Assert.That(ContainsDuplicates(_board.Hands), Is.False);
            });
        }

        [Test]
        public void DealWithNonEmptyBoard()
        {
            _board.Dealer = Seat.East;

            _board.Hand(Seat.West).Add(new Card(Rank.Ace, Suit.Clubs));
            _board.Hand(Seat.East).Add(new Card(Rank.Two, Suit.Clubs));
            _board.Hand(Seat.South).Add(new Card(Rank.Four, Suit.Diamonds));
            _board.Hand(Seat.South).Add(new Card(Rank.Five, Suit.Diamonds));

            _deck.Deal(_board);

            Assert.Multiple(() =>
            {
                Assert.That(_board.Hand(Seat.West).Count, Is.EqualTo(13));
                Assert.That(_board.Hand(Seat.East).Count, Is.EqualTo(13));
                Assert.That(_board.Hand(Seat.North).Count, Is.EqualTo(13));
                Assert.That(_board.Hand(Seat.South).Count, Is.EqualTo(13));

                Assert.That(ContainsDuplicates(_board.Hands), Is.False);
            });
        }

        [Test]
        public void DealRaisedDealtEvent()
        {
            var eventRaised = false;

            _deck.Dealt += (sender, args) => eventRaised = true;

            _board.Dealer = Seat.West;

            _deck.Deal(_board);

            Assert.That(eventRaised, Is.True);
        }

        private bool ContainsDuplicates()
        {
            return ContainsDuplicates(_deck);
        }

        private static bool ContainsDuplicates<T>(IEnumerable<T> enumerable)
        {
            var set = new HashSet<T>();
            return enumerable.Any(x => !set.Add(x));
        }
    }
}