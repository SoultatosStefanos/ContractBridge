using ContractBridge.Core;
using ContractBridge.Core.Impl;
using NUnit.Framework;

namespace ContractBridge.Tests.Core.Impl
{
    [TestFixture]
    [TestOf(typeof(Trick))]
    public class TrickTest
    {
        [SetUp]
        public void SetUp()
        {
            _trick = new Trick(new ICard[]
            {
                new Card(Rank.Ace, Suit.Clubs),
                new Card(Rank.Four, Suit.Diamonds),
                new Card(Rank.King, Suit.Spades),
                new Card(Rank.Five, Suit.Clubs)
            });
        }

        private Trick _trick;

        [Test]
        public void Enumeration()
        {
            Assert.DoesNotThrow(() =>
            {
                foreach (var card in _trick)
                {
                    Assert.That(card, Is.Not.Null);
                }
            });
        }
    }
}