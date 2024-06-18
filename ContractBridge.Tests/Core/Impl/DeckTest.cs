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
            _deck = new Deck(_cardFactory);
        }

        private readonly ICardFactory _cardFactory = new CardFactory();

        private IDeck _deck;

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
                Assert.That(_cardFactory.Create((Rank)2, Suit.Clubs), Is.EqualTo(card0));
                Assert.That(_cardFactory.Create((Rank)2, Suit.Diamonds), Is.EqualTo(card1));
                Assert.That(_cardFactory.Create(Rank.Two, Suit.Hearts), Is.EqualTo(card2));
                Assert.That(_cardFactory.Create(Rank.Two, Suit.Spades), Is.EqualTo(card3));
            });
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

        // TODO Test Deal

        private bool ContainsDuplicates()
        {
            var set = new HashSet<ICard>();
            return _deck.Any(card => !set.Add(card));
        }
    }
}