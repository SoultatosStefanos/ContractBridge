using ContractBridge.Core;
using ContractBridge.Core.Impl;
using NUnit.Framework;

namespace ContractBridge.Tests.Core.Impl
{
    [TestFixture]
    [TestOf(typeof(Hand))]
    public class HandTest
    {
        [SetUp]
        public void SetUp()
        {
            _hand = new Hand();
        }

        private Hand _hand;

        [Test]
        public void InitiallyIsEmpty()
        {
            Assert.That(_hand.IsEmpty(), Is.True);
        }

        [Test]
        public void InitiallyCountIsZero()
        {
            Assert.That(_hand.Count, Is.Zero);
        }

        [Test]
        public void AddThrowsCardAlreadyInHandExceptionWhenContainsCard()
        {
            var card = new Card(Rank.Eight, Suit.Clubs);

            _hand.Add(card);

            Assert.Throws<CardAlreadyInHandException>(() => { _hand.Add(card); });
        }

        [Test]
        public void AddOneThenIsNotEmpty()
        {
            _hand.Add(new Card(Rank.Eight, Suit.Clubs));

            Assert.That(_hand.IsEmpty(), Is.False);
        }

        [Test]
        public void AddOneThenCountIsOne()
        {
            _hand.Add(new Card(Rank.Eight, Suit.Clubs));

            Assert.That(_hand.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddTwoThenIsNotEmpty()
        {
            _hand.Add(new Card(Rank.Eight, Suit.Clubs));
            _hand.Add(new Card(Rank.Six, Suit.Spades));

            Assert.That(_hand.IsEmpty(), Is.False);
        }

        [Test]
        public void AddTwoThenCountIsTwo()
        {
            _hand.Add(new Card(Rank.Two, Suit.Diamonds));
            _hand.Add(new Card(Rank.Ace, Suit.Clubs));

            Assert.That(_hand.Count, Is.EqualTo(2));
        }

        [Test]
        public void AddRaisesAddedEvent()
        {
            var eventRaised = false;

            _hand.Added += (sender, args) => eventRaised = true;

            _hand.Add(new Card(Rank.Five, Suit.Spades));

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void AddThenAccessByIndex()
        {
            var card1 = new Card(Rank.Two, Suit.Diamonds);
            var card2 = new Card(Rank.Ace, Suit.Clubs);

            _hand.Add(card1);
            _hand.Add(card2);

            var card = _hand[1];

            Assert.That(card, Is.EqualTo(card2));
        }

        [Test]
        public void AddThenAccessByRankAndSuit()
        {
            var card1 = new Card(Rank.Two, Suit.Diamonds);
            var card2 = new Card(Rank.Ace, Suit.Clubs);

            _hand.Add(card1);
            _hand.Add(card2);

            var card = _hand[Rank.Ace, Suit.Clubs];

            Assert.That(card, Is.EqualTo(card2));
        }

        [Test]
        public void RemoveInitiallyDoesNotThrow()
        {
            Assert.DoesNotThrow(() => { _hand.Remove(new Card(Rank.Five, Suit.Diamonds)); });
        }

        [Test]
        public void IsEmptyAfterAddOneThenRemove()
        {
            var card = new Card(Rank.Ace, Suit.Clubs);
            _hand.Add(card);

            _hand.Remove(card);

            Assert.That(_hand.IsEmpty(), Is.True);
        }

        [Test]
        public void HasCountZeroAfterAddOneThenRemove()
        {
            var card = new Card(Rank.Ace, Suit.Clubs);
            _hand.Add(card);

            _hand.Remove(card);

            Assert.That(_hand.Count, Is.Zero);
        }

        [Test]
        public void IsNotEmptyAfterAddThreeThenRemoveTwo()
        {
            var card1 = new Card(Rank.Two, Suit.Clubs);
            var card2 = new Card(Rank.Three, Suit.Clubs);
            var card3 = new Card(Rank.Four, Suit.Clubs);

            _hand.Add(card1);
            _hand.Add(card2);
            _hand.Add(card3);

            _hand.Remove(card1);
            _hand.Remove(card2);

            Assert.That(_hand.IsEmpty(), Is.False);
        }

        [Test]
        public void HasCountOneAfterAddThreeThenRemoveTwo()
        {
            var card1 = new Card(Rank.Two, Suit.Clubs);
            var card2 = new Card(Rank.Three, Suit.Clubs);
            var card3 = new Card(Rank.Four, Suit.Clubs);

            _hand.Add(card1);
            _hand.Add(card2);
            _hand.Add(card3);

            _hand.Remove(card1);
            _hand.Remove(card2);

            Assert.That(_hand.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemoveRaisesRemovedEventWhenContainsCard()
        {
            var eventRaised = false;

            _hand.Removed += (sender, args) => eventRaised = true;

            var card = new Card(Rank.Five, Suit.Spades);
            _hand.Add(card);

            _hand.Remove(card);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void RemoveDoesNotRaiseRemovedEventWhenDoesNotContainCard()
        {
            var eventRaised = false;

            _hand.Removed += (sender, args) => eventRaised = true;

            _hand.Remove(new Card(Rank.Five, Suit.Spades));

            Assert.That(eventRaised, Is.False);
        }

        [Test]
        public void RemoveDoesNotRaiseEmptiedEventWhenIsEmpty()
        {
            var eventRaised = false;

            _hand.Emptied += (sender, args) => eventRaised = true;

            _hand.Remove(new Card(Rank.Three, Suit.Clubs));

            Assert.That(eventRaised, Is.False);
        }

        [Test]
        public void RemoveDoesNotRaiseEmptiedEventWhenIsNotEmptyAfter()
        {
            var eventRaised = false;

            _hand.Emptied += (sender, args) => eventRaised = true;

            var card1 = new Card(Rank.Two, Suit.Clubs);
            var card2 = new Card(Rank.Three, Suit.Clubs);

            _hand.Add(card1);
            _hand.Add(card2);

            _hand.Remove(card1);

            Assert.That(eventRaised, Is.False);
        }

        [Test]
        public void RemoveRaisesEmptiedEventWhenIsEmptyAfter()
        {
            var eventRaised = false;

            _hand.Emptied += (sender, args) => eventRaised = true;

            var card1 = new Card(Rank.Two, Suit.Clubs);
            var card2 = new Card(Rank.Three, Suit.Clubs);

            _hand.Add(card1);
            _hand.Add(card2);

            _hand.Remove(card1);
            _hand.Remove(card2);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void InitiallyClearDoesNotThrow()
        {
            Assert.DoesNotThrow(() => _hand.Clear());
        }

        [Test]
        public void ClearAfterTwoAddsThenIsEmpty()
        {
            _hand.Add(new Card(Rank.Two, Suit.Diamonds));
            _hand.Add(new Card(Rank.Ace, Suit.Clubs));

            _hand.Clear();

            Assert.That(_hand.IsEmpty(), Is.True);
        }

        [Test]
        public void ClearAfterTwoAddsThenCountIsZero()
        {
            _hand.Add(new Card(Rank.Two, Suit.Diamonds));
            _hand.Add(new Card(Rank.Ace, Suit.Clubs));

            _hand.Clear();

            Assert.That(_hand.Count, Is.Zero);
        }

        [Test]
        public void ClearRaisesClearedEvent()
        {
            var eventRaised = false;

            _hand.Cleared += (sender, args) => eventRaised = true;

            _hand.Clear();

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void ClearDoesNotRaiseEmptiedEventInitially()
        {
            var eventRaised = false;

            _hand.Emptied += (sender, args) => eventRaised = true;

            _hand.Clear();

            Assert.That(eventRaised, Is.False);
        }

        [Test]
        public void ClearAfterTwoAddsRaisesBothClearedAndEmptied()
        {
            var clearedEventRaised = false;
            var emptiedEventRaised = false;

            _hand.Cleared += (sender, args) => clearedEventRaised = true;
            _hand.Emptied += (sender, args) => emptiedEventRaised = true;

            _hand.Add(new Card(Rank.Two, Suit.Diamonds));
            _hand.Add(new Card(Rank.Ace, Suit.Clubs));

            _hand.Clear();

            Assert.That(clearedEventRaised, Is.True);
            Assert.That(emptiedEventRaised, Is.True);
        }
    }
}