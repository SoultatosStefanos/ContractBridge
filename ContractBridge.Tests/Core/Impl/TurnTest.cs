using ContractBridge.Core;
using ContractBridge.Core.Impl;
using NUnit.Framework;

namespace ContractBridge.Tests.Core.Impl
{
    [TestFixture]
    [TestOf(typeof(Turn))]
    public class TurnTest
    {
        [SetUp]
        public void SetUp()
        {
            _turn = new Turn(Seat.East);
        }

        private Turn _turn;

        [Test]
        public void IsNotPlayedInitially()
        {
            Assert.That(_turn.IsPlayed(), Is.False);
        }

        [Test]
        public void InitialSeatAccess()
        {
            Assert.That(_turn.Seat, Is.EqualTo(Seat.East));
        }

        [Test]
        public void MarkPlayedThenIsPlayed()
        {
            _turn.MarkPlayed();

            Assert.That(_turn.IsPlayed(), Is.True);
        }

        [Test]
        public void MarkPlayedTwiceThrowsTurnAlreadyPlayed()
        {
            _turn.MarkPlayed();

            Assert.Throws<TurnAlreadyPlayedException>(() => _turn.MarkPlayed());
        }

        [Test]
        public void MarkPlayedRaisesTurnPlayedException()
        {
            var eventRaised = false;

            _turn.MarkedPlayed += (sender, args) => eventRaised = true;

            _turn.MarkPlayed();

            Assert.That(eventRaised, Is.True);
        }
    }
}