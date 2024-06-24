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
            _auction = new Auction(new ContractFactory())
            {
                FirstTurn = Seat.East
            };
        }

        private Auction _auction;

        [Test]
        public void InitiallyBidsAreEmpty()
        {
            Assert.That(_auction.AllBids, Is.Empty);
        }

        [Test]
        public void CantCallOutOfTurn()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanCall(new Bid(Level.Five, Denomination.Clubs), Seat.North), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Five, Denomination.Clubs), Seat.West), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Five, Denomination.Clubs), Seat.South), Is.False);
            });
        }

        [Test]
        public void CantPassOutOfTurn()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanPass(Seat.North), Is.False);
                Assert.That(_auction.CanPass(Seat.West), Is.False);
                Assert.That(_auction.CanPass(Seat.South), Is.False);
            });
        }

        [Test]
        public void CantDoubleOutOfTurn()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanDouble(Seat.North), Is.False);
                Assert.That(_auction.CanDouble(Seat.West), Is.False);
                Assert.That(_auction.CanDouble(Seat.South), Is.False);
            });
        }

        [Test]
        public void CallOutOfTurnThrowsAuctionPlayOutOfTurnException()
        {
            Assert.Throws<AuctionPlayOutOfTurnException>(() =>
            {
                _auction.Call(new Bid(Level.Five, Denomination.NoTrumps), Seat.North);
            });
        }

        [Test]
        public void PassOutOfTurnThrowsAuctionPlayOutOfTurnException()
        {
            Assert.Throws<AuctionPlayOutOfTurnException>(() => { _auction.Pass(Seat.South); });
        }

        [Test]
        public void DoubleOutOfTurnThrowsAuctionPlayOutOfTurnException()
        {
            Assert.Throws<AuctionPlayOutOfTurnException>(() => { _auction.Pass(Seat.West); });
        }

        [Test]
        public void CantCallWithLowerOrSameDenomination()
        {
            _auction.Call(new Bid(Level.One, Denomination.NoTrumps), Seat.East);

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Clubs), Seat.South), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Diamonds), Seat.South), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Hearts), Seat.South), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Spades), Seat.South), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.NoTrumps), Seat.South), Is.False);
            });
        }

        [Test]
        public void CantCallWithLowerOrSameLevel()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Spades), Seat.East);

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanCall(new Bid(Level.Seven, Denomination.Spades), Seat.South), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Six, Denomination.Spades), Seat.South), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Five, Denomination.Spades), Seat.South), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Four, Denomination.Spades), Seat.South), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Three, Denomination.Spades), Seat.South), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.Two, Denomination.Spades), Seat.South), Is.False);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Spades), Seat.South), Is.False);
            });
        }

        [Test]
        public void CanCallWithHigherDenomination()
        {
            _auction.Call(new Bid(Level.One, Denomination.Clubs), Seat.East);

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Diamonds), Seat.South), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Hearts), Seat.South), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.Spades), Seat.South), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.One, Denomination.NoTrumps), Seat.South), Is.True);
            });
        }

        [Test]
        public void CanCallWithHigherLevel()
        {
            _auction.Call(new Bid(Level.One, Denomination.NoTrumps), Seat.East);

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanCall(new Bid(Level.Two, Denomination.Clubs), Seat.South), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Three, Denomination.Clubs), Seat.South), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Four, Denomination.Clubs), Seat.South), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Five, Denomination.Clubs), Seat.South), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Six, Denomination.Clubs), Seat.South), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Seven, Denomination.Clubs), Seat.South), Is.True);
            });
        }

        [Test]
        public void CanCallWithHigherLevelAndDenomination()
        {
            _auction.Call(new Bid(Level.One, Denomination.Clubs), Seat.East);

            Assert.Multiple(() =>
            {
                Assert.That(_auction.CanCall(new Bid(Level.Two, Denomination.Hearts), Seat.South), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Three, Denomination.Spades), Seat.South), Is.True);
                Assert.That(_auction.CanCall(new Bid(Level.Four, Denomination.NoTrumps), Seat.South), Is.True);
            });
        }

        [Test]
        public void CallWithLowerOrSameDenominationThrowsAuctionCallTooLowException()
        {
            _auction.Call(new Bid(Level.One, Denomination.NoTrumps), Seat.East);

            Assert.Multiple(() =>
            {
                Assert.Throws<AuctionCallTooLowException>(() =>
                {
                    _auction.Call(new Bid(Level.One, Denomination.Clubs), Seat.South);
                    _auction.Call(new Bid(Level.One, Denomination.Diamonds), Seat.South);
                    _auction.Call(new Bid(Level.One, Denomination.Hearts), Seat.South);
                    _auction.Call(new Bid(Level.One, Denomination.Spades), Seat.South);
                    _auction.Call(new Bid(Level.One, Denomination.NoTrumps), Seat.South);
                });
            });
        }

        [Test]
        public void CallWithLowerOrSameLevelThrowsAuctionCallTooLowException()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);

            Assert.Multiple(() =>
            {
                Assert.Throws<AuctionCallTooLowException>(() =>
                {
                    _auction.Call(new Bid(Level.One, Denomination.Clubs), Seat.South);
                    _auction.Call(new Bid(Level.Two, Denomination.Clubs), Seat.South);
                    _auction.Call(new Bid(Level.Three, Denomination.Clubs), Seat.South);
                    _auction.Call(new Bid(Level.Four, Denomination.Clubs), Seat.South);
                    _auction.Call(new Bid(Level.Five, Denomination.Clubs), Seat.South);
                    _auction.Call(new Bid(Level.Six, Denomination.Clubs), Seat.South);
                    _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.South);
                });
            });
        }

        [Test]
        public void CallThenCanPass()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);

            Assert.That(_auction.CanPass(Seat.South), Is.True);
        }

        [Test]
        public void CallThenPassFromAll()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);

            Assert.DoesNotThrow(() =>
            {
                _auction.Pass(Seat.South);
                _auction.Pass(Seat.West);
                _auction.Pass(Seat.North);
            });
        }

        [Test]
        public void CantDoubleOnPartner()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);
            _auction.Pass(Seat.South);

            Assert.That(_auction.CanDouble(Seat.West), Is.False);
        }

        [Test]
        public void DoubleOnPartnerThrowsAuctionDoubleOnPartnerException()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);
            _auction.Pass(Seat.South);

            Assert.Throws<AuctionDoubleOnPartnerException>(() => { _auction.Double(Seat.West); });
        }

        [Test]
        public void CantDoubleBeforeCall()
        {
            Assert.That(_auction.CanDouble(Seat.East), Is.False);
        }

        [Test]
        public void DoubleBeforeCallThrowsAuctionDoubleBeforeCallException()
        {
            Assert.Throws<AuctionDoubleBeforeCallException>(() => { _auction.Double(Seat.East); });
        }

        [Test]
        public void CanDoubleOnOpponent()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);

            Assert.That(_auction.CanDouble(Seat.South), Is.True);
        }

        [Test]
        public void DoubleOnOpponent()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);

            Assert.DoesNotThrow(() => { _auction.Double(Seat.South); });
        }

        [Test]
        public void CanReDoubleOnOpponent()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);

            _auction.Double(Seat.South);

            Assert.That(_auction.CanDouble(Seat.West), Is.True);
        }

        [Test]
        public void ReDoubleOnOpponent()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);

            _auction.Double(Seat.South);

            Assert.DoesNotThrow(() => { _auction.Double(Seat.West); });
        }

        [Test]
        public void CantReReDoubleOnOpponent()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);
            _auction.Double(Seat.South);
            _auction.Double(Seat.West);

            Assert.That(_auction.CanDouble(Seat.East), Is.False);
        }

        [Test]
        public void ReReDoubleThrowsAuctionReReDoubleException()
        {
            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);
            _auction.Double(Seat.South);
            _auction.Double(Seat.West);
            _auction.Pass(Seat.North);

            Assert.Throws<AuctionReReDoubleException>(() => { _auction.Double(Seat.East); });
        }

        [Test]
        public void CallRaisesCallEvent()
        {
            var eventRaised = false;
            _auction.Called += (sender, args) => eventRaised = true;

            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void PassRaisesPassEvent()
        {
            var eventRaised = false;
            _auction.Passed += (sender, args) => eventRaised = true;

            _auction.Pass(Seat.East);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void DoubleRaisesDoubledEvent()
        {
            var eventRaised = false;
            _auction.Doubled += (sender, args) => eventRaised = true;

            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);
            _auction.Double(Seat.South);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void RedoubleRaisesRedoubledEvent()
        {
            var eventRaised = false;
            _auction.Redoubled += (sender, args) => eventRaised = true;

            _auction.Call(new Bid(Level.Seven, Denomination.Clubs), Seat.East);
            _auction.Double(Seat.South);
            _auction.Double(Seat.West);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void CallThenThreePassesRaisesFinalContractMadeEvent()
        {
            var eventRaised = false;
            _auction.FinalContractMade += (sender, args) => eventRaised = true;

            _auction.Call(new Bid(Level.One, Denomination.Clubs), Seat.East);
            _auction.Pass(Seat.South);
            _auction.Pass(Seat.West);
            _auction.Pass(Seat.North);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void FinalContractOneBid()
        {
            _auction.Call(new Bid(Level.One, Denomination.Clubs), Seat.East);
            _auction.Pass(Seat.South);
            _auction.Pass(Seat.West);
            _auction.Pass(Seat.North);

            Assert.That(
                _auction.FinalContract,
                Is.EqualTo(new Contract(Level.One, Denomination.Clubs, Seat.East, null))
            );
        }

        [Test]
        public void FinalContractOverCall()
        {
            _auction.Call(new Bid(Level.One, Denomination.Clubs), Seat.East);
            _auction.Call(new Bid(Level.One, Denomination.Spades), Seat.South);
            _auction.Pass(Seat.West);
            _auction.Pass(Seat.North);
            _auction.Pass(Seat.East);

            Assert.That(
                _auction.FinalContract,
                Is.EqualTo(new Contract(Level.One, Denomination.Spades, Seat.South, null))
            );
        }

        [Test]
        public void FinalContractTwoOverCalls()
        {
            _auction.Call(new Bid(Level.One, Denomination.Clubs), Seat.East);
            _auction.Call(new Bid(Level.One, Denomination.Spades), Seat.South);
            _auction.Call(new Bid(Level.Two, Denomination.Spades), Seat.West);
            _auction.Pass(Seat.North);
            _auction.Pass(Seat.East);
            _auction.Pass(Seat.South);

            Assert.That(
                _auction.FinalContract,
                Is.EqualTo(new Contract(Level.Two, Denomination.Spades, Seat.West, null))
            );
        }

        [Test]
        public void FinalContractDoubled()
        {
            _auction.Call(new Bid(Level.One, Denomination.Hearts), Seat.East);
            _auction.Pass(Seat.South);
            _auction.Call(new Bid(Level.Four, Denomination.Hearts), Seat.West);
            _auction.Double(Seat.North);
            _auction.Pass(Seat.East);
            _auction.Pass(Seat.South);
            _auction.Pass(Seat.West);

            Assert.That(
                _auction.FinalContract,
                Is.EqualTo(new Contract(Level.Four, Denomination.Hearts, Seat.North, Risk.Doubled))
            );
        }

        [Test]
        public void FinalContractRedoubled()
        {
            _auction.Call(new Bid(Level.One, Denomination.Hearts), Seat.East);
            _auction.Double(Seat.South);
            _auction.Double(Seat.West);
            _auction.Pass(Seat.North);
            _auction.Pass(Seat.East);
            _auction.Pass(Seat.South);

            Assert.That(
                _auction.FinalContract,
                Is.EqualTo(new Contract(Level.One, Denomination.Hearts, Seat.West, Risk.Redoubled))
            );
        }

        [Test]
        public void PassedOut()
        {
            var eventRaised = false;
            _auction.PassedOut += (sender, args) => eventRaised = true;

            _auction.Pass(Seat.East);
            _auction.Pass(Seat.South);
            _auction.Pass(Seat.West);
            _auction.Pass(Seat.North);

            Assert.Multiple(() =>
            {
                Assert.That(eventRaised, Is.True);
                Assert.That(_auction.FinalContract, Is.Null);
            });
        }
    }
}