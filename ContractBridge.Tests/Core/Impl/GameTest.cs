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

            _game = new Game(_board, new TrickFactory())
            {
                TrumpSuit = TrumpSuit.Hearts,
                FirstLead = Seat.East
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
        public void InitiallyLeadAndTurnAreFirstLead()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_game.Lead, Is.EqualTo(Seat.East));
                Assert.That(_game.Turn, Is.EqualTo(Seat.East));
            });
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
        public void FollowOutOfTurnThrowsGamePlayOutOfTurnException()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<GamePlayOutOfTurnException>(() => _game.Follow(_board.Hand(Seat.North)[0], Seat.North));
                Assert.Throws<GamePlayOutOfTurnException>(() => _game.Follow(_board.Hand(Seat.South)[0], Seat.South));
                Assert.Throws<GamePlayOutOfTurnException>(() => _game.Follow(_board.Hand(Seat.West)[0], Seat.West));
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
        public void SetFirstLeadRaisedTurnAndLeadChangedEvent()
        {
            var turnChanged = false;
            var leadChanged = false;

            _game.TurnChanged += (sender, args) => turnChanged = true;
            _game.LeadChanged += (sender, args) => leadChanged = true;

            _game.FirstLead = Seat.North;

            Assert.Multiple(() =>
            {
                Assert.That(turnChanged, Is.EqualTo(true));
                Assert.That(leadChanged, Is.EqualTo(true));
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

        [Test]
        public void CanFollowWithAnyCardWhenCantFollowSuit()
        {
            _game.Follow(_board.Hand(Seat.East)[Rank.Two, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Three, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Four, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Five, Suit.Clubs], Seat.North);

            _game.Follow(_board.Hand(Seat.North)[Rank.Nine, Suit.Clubs], Seat.North);
            _game.Follow(_board.Hand(Seat.East)[Rank.Six, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Seven, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Eight, Suit.Clubs], Seat.West);

            _game.Follow(_board.Hand(Seat.North)[Rank.King, Suit.Clubs], Seat.North);
            _game.Follow(_board.Hand(Seat.East)[Rank.Ace, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Jack, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Queen, Suit.Clubs], Seat.West);

            _game.Follow(_board.Hand(Seat.East)[Rank.Ten, Suit.Clubs], Seat.East);

            Assert.Multiple(() =>
            {
                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.Two, Suit.Diamonds], Seat.South), Is.True);
                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.King, Suit.Hearts], Seat.South), Is.True);
                Assert.That(_game.CanFollow(_board.Hand(Seat.South)[Rank.Queen, Suit.Spades], Seat.South), Is.True);
            });
        }

        [Test]
        public void FollowWithAnyCardWhenCantFollowSuit()
        {
            _game.Follow(_board.Hand(Seat.East)[Rank.Two, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Three, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Four, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Five, Suit.Clubs], Seat.North);

            _game.Follow(_board.Hand(Seat.North)[Rank.Nine, Suit.Clubs], Seat.North);
            _game.Follow(_board.Hand(Seat.East)[Rank.Six, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Seven, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Eight, Suit.Clubs], Seat.West);

            _game.Follow(_board.Hand(Seat.North)[Rank.King, Suit.Clubs], Seat.North);
            _game.Follow(_board.Hand(Seat.East)[Rank.Ace, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Jack, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Queen, Suit.Clubs], Seat.West);

            _game.Follow(_board.Hand(Seat.East)[Rank.Ten, Suit.Clubs], Seat.East);

            Assert.DoesNotThrow(() => { _game.Follow(_board.Hand(Seat.South)[Rank.Two, Suit.Diamonds], Seat.South); });
        }

        [Test]
        public void FollowRaisesFollowedEvent()
        {
            var eventRaised = false;
            _game.Followed += (sender, args) => eventRaised = true;

            _game.Follow(_board.Hand(Seat.East)[Rank.Two, Suit.Clubs], Seat.East);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void TrickWonFollowSuit()
        {
            var eventRaised = false;

            _game.TrickWon += (sender, args) =>
            {
                eventRaised = true;

                var trick = args.Trick;
                var seat = args.Seat;

                Assert.Multiple(() =>
                {
                    Assert.That(seat, Is.EqualTo(Seat.East));

                    Assert.That(trick.Contains(_deck[Rank.Ace, Suit.Clubs]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Three, Suit.Clubs]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Four, Suit.Clubs]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Five, Suit.Clubs]), Is.True);
                });
            };

            _game.Follow(_board.Hand(Seat.East)[Rank.Ace, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Three, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Four, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Five, Suit.Clubs], Seat.North);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void TrickWonDiscarding()
        {
            _game.Follow(_board.Hand(Seat.East)[Rank.Six, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Three, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Four, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Five, Suit.Clubs], Seat.North);

            _game.Follow(_board.Hand(Seat.East)[Rank.Ten, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Seven, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Eight, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Nine, Suit.Clubs], Seat.North);

            _game.Follow(_board.Hand(Seat.East)[Rank.Ace, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Jack, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Queen, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.King, Suit.Clubs], Seat.North);

            var eventRaised = false;

            _game.TrickWon += (sender, args) =>
            {
                eventRaised = true;

                var trick = args.Trick;
                var seat = args.Seat;

                Assert.Multiple(() =>
                {
                    Assert.That(seat, Is.EqualTo(Seat.East));

                    Assert.That(trick.Contains(_deck[Rank.Two, Suit.Clubs]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Two, Suit.Diamonds]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Three, Suit.Diamonds]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Four, Suit.Diamonds]), Is.True);
                });
            };

            _game.Follow(_board.Hand(Seat.East)[Rank.Two, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Two, Suit.Diamonds], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Three, Suit.Diamonds], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Four, Suit.Diamonds], Seat.North);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void TrickWonTrumpingOneTrumpSuitCard()
        {
            _game.Follow(_board.Hand(Seat.East)[Rank.Six, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Three, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Four, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Five, Suit.Clubs], Seat.North);

            _game.Follow(_board.Hand(Seat.East)[Rank.Ten, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Seven, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Eight, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Nine, Suit.Clubs], Seat.North);

            _game.Follow(_board.Hand(Seat.East)[Rank.Ace, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Jack, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Queen, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.King, Suit.Clubs], Seat.North);

            var eventRaised = false;

            _game.TrickWon += (sender, args) =>
            {
                eventRaised = true;

                var trick = args.Trick;
                var seat = args.Seat;

                Assert.Multiple(() =>
                {
                    Assert.That(seat, Is.EqualTo(Seat.North));

                    Assert.That(trick.Contains(_deck[Rank.Two, Suit.Clubs]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Two, Suit.Diamonds]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Three, Suit.Diamonds]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Three, Suit.Hearts]), Is.True);
                });
            };

            _game.Follow(_board.Hand(Seat.East)[Rank.Two, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Two, Suit.Diamonds], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Three, Suit.Diamonds], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Three, Suit.Hearts], Seat.North);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void TrickWonTrumpingTwoTrumpSuitCards()
        {
            _game.Follow(_board.Hand(Seat.East)[Rank.Six, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Three, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Four, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Five, Suit.Clubs], Seat.North);

            _game.Follow(_board.Hand(Seat.East)[Rank.Ten, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Seven, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Eight, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Nine, Suit.Clubs], Seat.North);

            _game.Follow(_board.Hand(Seat.East)[Rank.Ace, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Jack, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Queen, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.King, Suit.Clubs], Seat.North);

            var eventRaised = false;

            _game.TrickWon += (sender, args) =>
            {
                eventRaised = true;

                var trick = args.Trick;
                var seat = args.Seat;

                Assert.Multiple(() =>
                {
                    Assert.That(seat, Is.EqualTo(Seat.West));

                    Assert.That(trick.Contains(_deck[Rank.Two, Suit.Clubs]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Two, Suit.Diamonds]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Six, Suit.Hearts]), Is.True);
                    Assert.That(trick.Contains(_deck[Rank.Three, Suit.Hearts]), Is.True);
                });
            };

            _game.Follow(_board.Hand(Seat.East)[Rank.Two, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Two, Suit.Diamonds], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Six, Suit.Hearts], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Three, Suit.Hearts], Seat.North);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void TrickWinnerLeadsNextTrick()
        {
            _game.Follow(_board.Hand(Seat.East)[Rank.Six, Suit.Clubs], Seat.East);
            _game.Follow(_board.Hand(Seat.South)[Rank.Three, Suit.Clubs], Seat.South);
            _game.Follow(_board.Hand(Seat.West)[Rank.Four, Suit.Clubs], Seat.West);
            _game.Follow(_board.Hand(Seat.North)[Rank.Nine, Suit.Clubs], Seat.North);

            Assert.Multiple(() =>
            {
                Assert.That(_game.Lead, Is.EqualTo(Seat.North));
                Assert.That(_game.Turn, Is.EqualTo(Seat.North));
            });
        }
    }
}