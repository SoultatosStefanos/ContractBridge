using ContractBridge.Core;
using ContractBridge.Core.Impl;
using NUnit.Framework;

namespace ContractBridge.Tests.Core.Impl
{
    [TestFixture]
    [TestOf(typeof(Auction))]
    public class AuctionTest
    {
        [SetUp]
        public void SetUp()
        {
            _auction = new Auction(new Board(new HandFactory()), new ContractFactory());
        }

        private Auction _auction;

        [Test]
        public void CantCallWithPlayedTurn()
        {
            var turn = new Turn(Seat.East);

            turn.MarkPlayed();

            Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Clubs), turn), Is.False);
        }

        [Test]
        public void CantPassWithPlayedTurn()
        {
            var turn = new Turn(Seat.East);

            turn.MarkPlayed();

            Assert.That(_auction.CanPass(turn), Is.False);
        }

        [Test]
        public void CantDoubleWithPlayedTurn()
        {
            var turn = new Turn(Seat.East);

            turn.MarkPlayed();

            Assert.That(_auction.CanDouble(turn), Is.False);
        }

        [Test]
        public void CallWithPlayedTurnThrowsAuctionTurnAlreadyPlayedException()
        {
            var turn = new Turn(Seat.East);

            turn.MarkPlayed();

            Assert.Throws<AuctionTurnAlreadyPlayedException>(() =>
            {
                _auction.Call(new Bid(Level.One, Denomination.Clubs), turn);
            });
        }

        [Test]
        public void PassWithPlayedTurnThrowsAuctionTurnAlreadyPlayedException()
        {
            var turn = new Turn(Seat.East);

            turn.MarkPlayed();

            Assert.Throws<AuctionTurnAlreadyPlayedException>(() => { _auction.Pass(turn); });
        }

        [Test]
        public void DoubleWithPlayedTurnThrowsAuctionTurnAlreadyPlayedException()
        {
            var turn = new Turn(Seat.East);

            turn.MarkPlayed();

            Assert.Throws<AuctionTurnAlreadyPlayedException>(() => { _auction.Double(turn); });
        }

        [Test]
        public void CantCallSelf()
        {
            _auction.Call(new Bid(Level.One, Denomination.Spades), new Turn(Seat.East));

            Assert.That(_auction.CanCall(new Bid(Level.Four, Denomination.Spades), new Turn(Seat.East)), Is.False);

            // Assert.Throws<AuctionPlayAgainstSelfException>(() =>
            // {
            //     _auction.Call(new Bid(Level.Four, Denomination.Spades), new Turn(Seat.East));
            // });
        }

        [Test]
        public void CantPassOnSelf()
        {
            _auction.Call(new Bid(Level.One, Denomination.Spades), new Turn(Seat.East));

            Assert.That(_auction.CanPass(new Turn(Seat.East)), Is.False);
        }

        [Test]
        public void CantDoubleOnSelf()
        {
            _auction.Call(new Bid(Level.One, Denomination.Spades), new Turn(Seat.East));

            Assert.That(_auction.CanDouble(new Turn(Seat.East)), Is.False);
        }

        [Test]
        public void CallOnSelfThrowsAuctionPlayAgainstSelfException()
        {
            _auction.Call(new Bid(Level.One, Denomination.Spades), new Turn(Seat.East));

            Assert.Throws<AuctionPlayAgainstSelfException>(() =>
            {
                _auction.Call(new Bid(Level.Four, Denomination.Spades), new Turn(Seat.East));
            });
        }

        [Test]
        public void PassOnSelfThrowsAuctionPlayAgainstSelfException()
        {
            _auction.Call(new Bid(Level.One, Denomination.Spades), new Turn(Seat.East));

            Assert.Throws<AuctionPlayAgainstSelfException>(() => { _auction.Pass(new Turn(Seat.East)); });
        }

        [Test]
        public void DoubleOnSelfThrowsAuctionPlayAgainstSelfException()
        {
            _auction.Call(new Bid(Level.One, Denomination.Spades), new Turn(Seat.East));

            Assert.Throws<AuctionPlayAgainstSelfException>(() => { _auction.Double(new Turn(Seat.East)); });
        }

        [Test]
        public void CantCallWithLowerOrSameDenomination()
        {
            _auction.Call(new Bid(Level.One, Denomination.NoTrumps), new Turn(Seat.East));

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Clubs), new Turn(Seat.West)), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Diamonds), new Turn(Seat.West)), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Hearts), new Turn(Seat.West)), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Spades), new Turn(Seat.West)), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.NoTrumps), new Turn(Seat.West)), Is.False);
            });
        }

        [Test]
        public void CantCallWithLowerOrSameLevel()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Spades), new Turn(Seat.East));

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanCall(new Bid(Level.Seven, Denomination.Spades), new Turn(Seat.West)), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Six, Denomination.Spades), new Turn(Seat.West)), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Five, Denomination.Spades), new Turn(Seat.West)), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Four, Denomination.Spades), new Turn(Seat.West)), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Three, Denomination.Spades), new Turn(Seat.West)), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Two, Denomination.Spades), new Turn(Seat.West)), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Spades), new Turn(Seat.West)), Is.False);
            });
        }

        [Test]
        public void CanCallWithHigherDenomination()
        {
            _auction.Call(new Bid(Level.One, Denomination.Clubs), new Turn(Seat.East));

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Diamonds), new Turn(Seat.North)), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Hearts), new Turn(Seat.West)), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Spades), new Turn(Seat.South)), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.NoTrumps), new Turn(Seat.North)), Is.True);
            });
        }

        [Test]
        public void CanCallWithHigherLevel()
        {
            _auction.Call(new Bid(Level.One, Denomination.NoTrumps), new Turn(Seat.East));

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanCall(new Bid(Level.Two, Denomination.Clubs), new Turn(Seat.North)), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Three, Denomination.Clubs), new Turn(Seat.North)), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Four, Denomination.Clubs), new Turn(Seat.West)), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Five, Denomination.Clubs), new Turn(Seat.North)), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Six, Denomination.Clubs), new Turn(Seat.West)), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Seven, Denomination.Clubs), new Turn(Seat.South)), Is.True);
            });
        }

        [Test]
        public void CanCallWithHigherLevelAndDenomination()
        {
            _auction.Call(new Bid(Level.One, Denomination.Clubs), new Turn(Seat.East));

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanCall(new Bid(Level.Two, Denomination.Hearts), new Turn(Seat.West)), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Three, Denomination.Spades), new Turn(Seat.North)), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Four, Denomination.NoTrumps), new Turn(Seat.West)), Is.True);
            });
        }

        [Test]
        public void CallWithLowerOrSameDenominationThrowsAuctionCallTooLowException()
        {
            _auction.Call(new Bid(Level.One, Denomination.NoTrumps), new Turn(Seat.East));

            Assert.Multiple(() =>
            {
                Assert.Throws<AuctionCallTooLowException>(() =>
                {
                    _auction.Call(new Bid(Level.One, Denomination.Clubs), new Turn(Seat.West));
                    _auction.Call(new Bid(Level.One, Denomination.Diamonds), new Turn(Seat.West));
                    _auction.Call(new Bid(Level.One, Denomination.Hearts), new Turn(Seat.West));
                    _auction.Call(new Bid(Level.One, Denomination.Spades), new Turn(Seat.West));
                    _auction.Call(new Bid(Level.One, Denomination.NoTrumps), new Turn(Seat.West));
                });
            });
        }

        [Test]
        public void CallWithLowerOrSameLevelThrowsAuctionCallTooLowException()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), new Turn(Seat.East));

            Assert.Multiple(() =>
            {
                Assert.Throws<AuctionCallTooLowException>(() =>
                {
                    _auction.Call(new Bid(Level.One, Denomination.Clubs), new Turn(Seat.West));
                    _auction.Call(new Bid(Level.Two, Denomination.Clubs), new Turn(Seat.West));
                    _auction.Call(new Bid(Level.Three, Denomination.Clubs), new Turn(Seat.West));
                    _auction.Call(new Bid(Level.Four, Denomination.Clubs), new Turn(Seat.West));
                    _auction.Call(new Bid(Level.Five, Denomination.Clubs), new Turn(Seat.West));
                    _auction.Call(new Bid(Level.Six, Denomination.Clubs), new Turn(Seat.West));
                    _auction.Call(new Bid(Level.Seven, Denomination.Clubs), new Turn(Seat.West));
                });
            });
        }

        [Test]
        public void CallThenCanPassFromAll()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), new Turn(Seat.East));

            Assert.Multiple(() =>
            {
                _auction.CanPass(new Turn(Seat.West));
                _auction.CanPass(new Turn(Seat.North));
                _auction.CanPass(new Turn(Seat.South));
            });
        }

        [Test]
        public void CallThenPassFromAll()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), new Turn(Seat.East));

            Assert.DoesNotThrow(() =>
            {
                _auction.Pass(new Turn(Seat.West));
                _auction.Pass(new Turn(Seat.North));
                _auction.Pass(new Turn(Seat.South));
            });
        }

        [Test]
        public void CallThenCantCallWithSameTurn()
        {
            var turn = new Turn(Seat.East);

            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), turn);

            Assert.That(_auction.CanCall(new Bid(Level.Seven, Denomination.NoTrumps), turn), Is.False);
        }

        [Test]
        public void CallThenCantPassWithSameTurn()
        {
            var turn = new Turn(Seat.East);

            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), turn);

            Assert.That(_auction.CanPass(turn), Is.False);
        }

        [Test]
        public void CallThenCantDoubleWithSameTurn()
        {
            var turn = new Turn(Seat.East);

            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), turn);

            Assert.That(_auction.CanDouble(turn), Is.False);
        }

        [Test]
        public void CallThenCallWithSameTurnThrowsAuctionTurnAlreadyPlayedException()
        {
            var turn = new Turn(Seat.East);

            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), turn);

            Assert.Throws<AuctionTurnAlreadyPlayedException>(() =>
            {
                _auction.Call(new Bid(Level.Seven, Denomination.NoTrumps), turn);
            });
        }

        [Test]
        public void CallThenPassWithSameTurnThrowsAuctionTurnAlreadyPlayedException()
        {
            var turn = new Turn(Seat.East);

            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), turn);

            Assert.Throws<AuctionTurnAlreadyPlayedException>(() => { _auction.Pass(turn); });
        }

        [Test]
        public void CallThenDoubleWithSameTurnThrowsAuctionTurnAlreadyPlayedException()
        {
            var turn = new Turn(Seat.East);

            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), turn);

            Assert.Throws<AuctionTurnAlreadyPlayedException>(() => { _auction.Double(turn); });
        }

        [Test]
        public void CantDoubleOnPartner()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), new Turn(Seat.East));

            Assert.That(_auction.CanDouble(new Turn(Seat.West)), Is.False);
        }

        [Test]
        public void DoubleOnPartnerThrowsAuctionDoubleOnPartnerException()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), new Turn(Seat.East));

            Assert.Throws<AuctionDoubleOnPartnerException>(() => { _auction.Double(new Turn(Seat.West)); });
        }

        [Test]
        public void CantDoubleBeforeCall()
        {
            Assert.That(_auction.CanDouble(new Turn(Seat.West)), Is.False);
        }

        [Test]
        public void CanDoubleOnOpponent()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), new Turn(Seat.East));

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanDouble(new Turn(Seat.North)), Is.True);
                Assert.That(_auction.CanDouble(new Turn(Seat.South)), Is.True);
            });
        }

        [Test]
        public void DoubleOnOpponent()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), new Turn(Seat.East));

            Assert.DoesNotThrow(() => { _auction.Double(new Turn(Seat.North)); });
        }

        [Test]
        public void CanReDoubleOnOpponent()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), new Turn(Seat.East));

            _auction.Double(new Turn(Seat.North));

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanDouble(new Turn(Seat.East)), Is.True);
                Assert.That(_auction.CanDouble(new Turn(Seat.West)), Is.True);
            });
        }

        [Test]
        public void ReDoubleOnOpponent()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), new Turn(Seat.East));

            _auction.Double(new Turn(Seat.North));

            Assert.DoesNotThrow(() => { _auction.Double(new Turn(Seat.East)); });
        }
    }
}