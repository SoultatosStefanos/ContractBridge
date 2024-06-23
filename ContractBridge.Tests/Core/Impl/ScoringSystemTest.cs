using ContractBridge.Core;
using ContractBridge.Core.Impl;
using NUnit.Framework;

// ReSharper disable ConvertToConstant.Local

namespace ContractBridge.Tests.Core.Impl
{
    [TestFixture]
    [TestOf(typeof(ScoringSystem))]
    public class ScoringSystemTest
    {
        [SetUp]
        public void SetUp()
        {
            _scoringSystem = new ScoringSystem();
        }

        private ScoringSystem _scoringSystem;

        [Test]
        public void SuccessfulMinorSuit()
        {
            var contract = new Contract(Level.Three, Denomination.Diamonds, Seat.East, null);
            var vulnerable = false;
            var tricksMade = 9;

            var (declarerScore, defenderScore) = _scoringSystem.Score(contract, vulnerable, tricksMade);

            Assert.Multiple(() =>
            {
                Assert.That(declarerScore, Is.EqualTo(110));
                Assert.That(defenderScore, Is.EqualTo(0));
            });
        }

        [Test]
        public void SuccessfulMinorSuitOneOverTrick()
        {
            var contract = new Contract(Level.Three, Denomination.Diamonds, Seat.East, null);
            var vulnerable = false;
            var tricksMade = 10;

            var (declarerScore, defenderScore) = _scoringSystem.Score(contract, vulnerable, tricksMade);

            Assert.Multiple(() =>
            {
                Assert.That(declarerScore, Is.EqualTo(130));
                Assert.That(defenderScore, Is.EqualTo(0));
            });
        }

        [Test]
        public void SuccessfulMajorSuitVulnerable()
        {
            var contract = new Contract(Level.Four, Denomination.Spades, Seat.East, null);
            var vulnerable = true;
            var tricksMade = 10;

            var (declarerScore, defenderScore) = _scoringSystem.Score(contract, vulnerable, tricksMade);

            Assert.Multiple(() =>
            {
                Assert.That(declarerScore, Is.EqualTo(620));
                Assert.That(defenderScore, Is.EqualTo(0));
            });
        }

        [Test]
        public void SuccessfulMajorSuitVulnerableOvertricks()
        {
            var contract = new Contract(Level.Four, Denomination.Spades, Seat.East, null);
            var vulnerable = true;
            var tricksMade = 12;

            var (declarerScore, defenderScore) = _scoringSystem.Score(contract, vulnerable, tricksMade);

            Assert.Multiple(() =>
            {
                Assert.That(declarerScore, Is.EqualTo(680));
                Assert.That(defenderScore, Is.EqualTo(0));
            });
        }

        [Test]
        public void NotMakingMinorSuit()
        {
            var contract = new Contract(Level.Two, Denomination.Clubs, Seat.East, null);
            var vulnerable = false;
            var tricksMade = 4;

            var (declarerScore, defenderScore) = _scoringSystem.Score(contract, vulnerable, tricksMade);

            Assert.Multiple(() =>
            {
                Assert.That(declarerScore, Is.EqualTo(0));
                Assert.That(defenderScore, Is.EqualTo(100));
            });
        }

        [Test]
        public void DoubledContract()
        {
            var contract = new Contract(Level.Three, Denomination.Hearts, Seat.East, Risk.Doubled);
            var vulnerable = false;
            var tricksMade = 10;

            var (declarerScore, defenderScore) = _scoringSystem.Score(contract, vulnerable, tricksMade);

            Assert.Multiple(() =>
            {
                Assert.That(declarerScore, Is.EqualTo(380));
                Assert.That(defenderScore, Is.EqualTo(0));
            });
        }

        [Test]
        public void SlamBonusVulnerable()
        {
            var contract = new Contract(Level.Six, Denomination.NoTrumps, Seat.East, null);
            var vulnerable = true;
            var tricksMade = 13;

            var (declarerScore, defenderScore) = _scoringSystem.Score(contract, vulnerable, tricksMade);

            Assert.Multiple(() =>
            {
                Assert.That(declarerScore, Is.EqualTo(970));
                Assert.That(defenderScore, Is.EqualTo(0));
            });
        }

        [Test]
        public void GrandSlamNotMade()
        {
            var contract = new Contract(Level.Seven, Denomination.Spades, Seat.East, null);
            var vulnerable = true;
            var tricksMade = 12;

            var (declarerScore, defenderScore) = _scoringSystem.Score(contract, vulnerable, tricksMade);

            Assert.Multiple(() =>
            {
                Assert.That(declarerScore, Is.EqualTo(0));
                Assert.That(defenderScore, Is.EqualTo(100));
            });
        }

        [Test]
        public void SuccessfulNoTrumpWithOvertricks()
        {
            var contract = new Contract(Level.Three, Denomination.NoTrumps, Seat.East, null);
            var vulnerable = false;
            var tricksMade = 1;

            var (declarerScore, defenderScore) = _scoringSystem.Score(contract, vulnerable, tricksMade);

            Assert.Multiple(() =>
            {
                Assert.That(declarerScore, Is.EqualTo(460));
                Assert.That(defenderScore, Is.EqualTo(0));
            });
        }
    }
}