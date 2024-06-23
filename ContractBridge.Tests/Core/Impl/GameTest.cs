using ContractBridge.Core;
using ContractBridge.Core.Impl;
using NUnit.Framework;

namespace ContractBridge.Tests.Core.Impl
{
    [TestFixture]
    [TestOf(typeof(Game))]
    public class GameTest
    {
        [SetUp]
        public void SetUp()
        {
            _board = new Board(new HandFactory())
            {
                Dealer = Seat.North
            };

            _deck = new Deck(new CardFactory(), IDeck.Partition.BySuit);
            _deck.Deal(_board);

            _game = new Game(
                _board,
                new TurnPlayContext(new TurnSequence(new TurnFactory())
                {
                    Lead = Seat.East
                })
            )
            {
                TrumpSuit = TrumpSuit.Hearts
            };
        }

        private Game _game;

        private IBoard _board;

        private IDeck _deck;

        [Test]
        public void InitiallyPlayedCardsAreEmpty()
        {
            Assert.That(_game.AllPlayedCards, Is.Empty);
        }

        [Test]
        public void CantFollowOutOfTurn()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_game.CanFollow(Seat.North), Is.False);
                Assert.That(_game.CanFollow(Seat.West), Is.False);
                Assert.That(_game.CanFollow(Seat.South), Is.False);

                Assert.That(_game.CanFollow(_board.Hand(Seat.North)[0], Seat.North), Is.False);
                Assert.That(_game.CanFollow(_board.Hand(Seat.West)[0], Seat.West), Is.False);
                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[0], Seat.South), Is.False);
            });
        }

        [Test]
        public void FollowOutOfTurnThrowsPlayOutOfTurnException()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<PlayOutOfTurnException>(() => _game.Follow(_board.Hand(Seat.North)[0], Seat.North));
                Assert.Throws<PlayOutOfTurnException>(() => _game.Follow(_board.Hand(Seat.South)[0], Seat.South));
                Assert.Throws<PlayOutOfTurnException>(() => _game.Follow(_board.Hand(Seat.West)[0], Seat.West));
            });
        }

        [Test]
        public void InitiallyLeadCanFollowWithAnyCard()
        {
            Assert.That(_game.CanFollow(Seat.East), Is.True);
        }

        [Test]
        public void InitiallyFollowWithAnyCard()
        {
            Assert.DoesNotThrow(() => _game.Follow(_board.Hand(Seat.East)[0], Seat.East));
        }

        [Test]
        public void CanFollowWithCardOutOfHandThrowsCardNotInHandException()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<CardNotInHandException>(() =>
                    _game.CanFollow(_deck[Rank.Four, Suit.Clubs], Seat.South)
                );

                Assert.Throws<CardNotInHandException>(() =>
                    _game.CanFollow(_deck[Rank.Five, Suit.Clubs], Seat.South)
                );

                Assert.Throws<CardNotInHandException>(() =>
                    _game.CanFollow(_deck[Rank.Two, Suit.Clubs], Seat.South)
                );
            });
        }

        [Test]
        public void FollowWithCardOutOfHandThrowsCardNotInHandException()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<CardNotInHandException>(() =>
                    _game.Follow(_deck[Rank.Four, Suit.Clubs], Seat.South)
                );

                Assert.Throws<CardNotInHandException>(() =>
                    _game.Follow(_deck[Rank.Five, Suit.Clubs], Seat.South)
                );

                Assert.Throws<CardNotInHandException>(() =>
                    _game.Follow(_deck[Rank.Two, Suit.Clubs], Seat.South)
                );
            });
        }

        [Test]
        public void CantFollowWithOtherSuitWhenCanFollowSuit()
        {
            _game.Follow(_board.Hand(Seat.East)[Rank.Two, Suit.Clubs], Seat.East);

            Assert.Multiple(() =>
            {
                Assert.That(_game.CanFollow(Seat.South), Is.True);

                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.Two, Suit.Diamonds], Seat.South), Is.False);
                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.Six, Suit.Diamonds], Seat.South), Is.False);
                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.Ten, Suit.Diamonds], Seat.South), Is.False);
                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.Ace, Suit.Diamonds], Seat.South), Is.False);

                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.Five, Suit.Hearts], Seat.South), Is.False);
                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.Nine, Suit.Hearts], Seat.South), Is.False);
                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.King, Suit.Hearts], Seat.South), Is.False);

                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.Four, Suit.Spades], Seat.South), Is.False);
                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.Eight, Suit.Spades], Seat.South), Is.False);
                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.Queen, Suit.Spades], Seat.South), Is.False);
            });
        }
    }
}