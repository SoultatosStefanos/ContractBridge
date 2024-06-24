using ContractBridge.Core;
using ContractBridge.Core.Impl;
using NUnit.Framework;

namespace ContractBridge.Tests.Core.Impl
{
    [TestFixture]
    [TestOf(typeof(Card))]
    public class CardTest
    {
        [Test]
        public void ToPbn()
        {
            Assert.That("S2", Is.EqualTo(new Card(Rank.Two, Suit.Spades).ToPbn()));
            Assert.That("S4", Is.EqualTo(new Card(Rank.Four, Suit.Spades).ToPbn()));
            Assert.That("S3", Is.EqualTo(new Card(Rank.Three, Suit.Spades).ToPbn()));
            Assert.That("S5", Is.EqualTo(new Card(Rank.Five, Suit.Spades).ToPbn()));
            Assert.That("S6", Is.EqualTo(new Card(Rank.Six, Suit.Spades).ToPbn()));
            Assert.That("S7", Is.EqualTo(new Card(Rank.Seven, Suit.Spades).ToPbn()));
            Assert.That("S8", Is.EqualTo(new Card(Rank.Eight, Suit.Spades).ToPbn()));
            Assert.That("S9", Is.EqualTo(new Card(Rank.Nine, Suit.Spades).ToPbn()));
            Assert.That("ST", Is.EqualTo(new Card(Rank.Ten, Suit.Spades).ToPbn()));
            Assert.That("SJ", Is.EqualTo(new Card(Rank.Jack, Suit.Spades).ToPbn()));
            Assert.That("SQ", Is.EqualTo(new Card(Rank.Queen, Suit.Spades).ToPbn()));
            Assert.That("SK", Is.EqualTo(new Card(Rank.King, Suit.Spades).ToPbn()));
            Assert.That("SA", Is.EqualTo(new Card(Rank.Ace, Suit.Spades).ToPbn()));

            Assert.That("H2", Is.EqualTo(new Card(Rank.Two, Suit.Hearts).ToPbn()));
            Assert.That("H3", Is.EqualTo(new Card(Rank.Three, Suit.Hearts).ToPbn()));
            Assert.That("H4", Is.EqualTo(new Card(Rank.Four, Suit.Hearts).ToPbn()));
            Assert.That("H5", Is.EqualTo(new Card(Rank.Five, Suit.Hearts).ToPbn()));
            Assert.That("H6", Is.EqualTo(new Card(Rank.Six, Suit.Hearts).ToPbn()));
            Assert.That("H7", Is.EqualTo(new Card(Rank.Seven, Suit.Hearts).ToPbn()));
            Assert.That("H8", Is.EqualTo(new Card(Rank.Eight, Suit.Hearts).ToPbn()));
            Assert.That("H9", Is.EqualTo(new Card(Rank.Nine, Suit.Hearts).ToPbn()));
            Assert.That("HT", Is.EqualTo(new Card(Rank.Ten, Suit.Hearts).ToPbn()));
            Assert.That("HJ", Is.EqualTo(new Card(Rank.Jack, Suit.Hearts).ToPbn()));
            Assert.That("HQ", Is.EqualTo(new Card(Rank.Queen, Suit.Hearts).ToPbn()));
            Assert.That("HK", Is.EqualTo(new Card(Rank.King, Suit.Hearts).ToPbn()));
            Assert.That("HA", Is.EqualTo(new Card(Rank.Ace, Suit.Hearts).ToPbn()));

            Assert.That("D2", Is.EqualTo(new Card(Rank.Two, Suit.Diamonds).ToPbn()));
            Assert.That("D3", Is.EqualTo(new Card(Rank.Three, Suit.Diamonds).ToPbn()));
            Assert.That("D4", Is.EqualTo(new Card(Rank.Four, Suit.Diamonds).ToPbn()));
            Assert.That("D5", Is.EqualTo(new Card(Rank.Five, Suit.Diamonds).ToPbn()));
            Assert.That("D6", Is.EqualTo(new Card(Rank.Six, Suit.Diamonds).ToPbn()));
            Assert.That("D7", Is.EqualTo(new Card(Rank.Seven, Suit.Diamonds).ToPbn()));
            Assert.That("D8", Is.EqualTo(new Card(Rank.Eight, Suit.Diamonds).ToPbn()));
            Assert.That("D9", Is.EqualTo(new Card(Rank.Nine, Suit.Diamonds).ToPbn()));
            Assert.That("DT", Is.EqualTo(new Card(Rank.Ten, Suit.Diamonds).ToPbn()));
            Assert.That("DJ", Is.EqualTo(new Card(Rank.Jack, Suit.Diamonds).ToPbn()));
            Assert.That("DQ", Is.EqualTo(new Card(Rank.Queen, Suit.Diamonds).ToPbn()));
            Assert.That("DK", Is.EqualTo(new Card(Rank.King, Suit.Diamonds).ToPbn()));
            Assert.That("DA", Is.EqualTo(new Card(Rank.Ace, Suit.Diamonds).ToPbn()));

            Assert.That("C2", Is.EqualTo(new Card(Rank.Two, Suit.Clubs).ToPbn()));
            Assert.That("C3", Is.EqualTo(new Card(Rank.Three, Suit.Clubs).ToPbn()));
            Assert.That("C4", Is.EqualTo(new Card(Rank.Four, Suit.Clubs).ToPbn()));
            Assert.That("C5", Is.EqualTo(new Card(Rank.Five, Suit.Clubs).ToPbn()));
            Assert.That("C6", Is.EqualTo(new Card(Rank.Six, Suit.Clubs).ToPbn()));
            Assert.That("C7", Is.EqualTo(new Card(Rank.Seven, Suit.Clubs).ToPbn()));
            Assert.That("C8", Is.EqualTo(new Card(Rank.Eight, Suit.Clubs).ToPbn()));
            Assert.That("C9", Is.EqualTo(new Card(Rank.Nine, Suit.Clubs).ToPbn()));
            Assert.That("CT", Is.EqualTo(new Card(Rank.Ten, Suit.Clubs).ToPbn()));
            Assert.That("CJ", Is.EqualTo(new Card(Rank.Jack, Suit.Clubs).ToPbn()));
            Assert.That("CQ", Is.EqualTo(new Card(Rank.Queen, Suit.Clubs).ToPbn()));
            Assert.That("CK", Is.EqualTo(new Card(Rank.King, Suit.Clubs).ToPbn()));
            Assert.That("CA", Is.EqualTo(new Card(Rank.Ace, Suit.Clubs).ToPbn()));
        }
    }
}