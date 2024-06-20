using System;
using ContractBridge.Core;
using ContractBridge.Core.Impl;
using NUnit.Framework;

namespace ContractBridge.Tests.Core.Impl
{
    [TestFixture]
    [TestOf(typeof(TurnSequence))]
    public class TurnSequenceTest
    {
        [SetUp]
        public void SetUp()
        {
            _turnSequence = new TurnSequence(new TurnFactory());
        }

        private TurnSequence _turnSequence;

        [Test]
        public void InitiallyHasNoLead()
        {
            Assert.That(_turnSequence.Lead, Is.Null);
        }

        [Test]
        public void SetLeadThenHasLead()
        {
            _turnSequence.Lead = Seat.West;

            Assert.That(_turnSequence.Lead, Is.EqualTo(Seat.West));
        }

        [Test]
        public void SetLeadRaisesLeadSetEvent()
        {
            var eventRaised = false;

            _turnSequence.LeadSet += (sender, args) => eventRaised = true;

            _turnSequence.Lead = Seat.West;

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void SetLeadWithNullThrowsNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => _turnSequence.Lead = null);
        }

        [Test]
        public void NextTurnAfterLeadSetBelongsToLead()
        {
            _turnSequence.Lead = Seat.South;

            var nextTurn = _turnSequence.NextTurn();

            Assert.Multiple(() =>
            {
                Assert.That(nextTurn, Is.Not.Null);
                Assert.That(nextTurn.Seat, Is.EqualTo(Seat.South));
            });
        }

        [Test]
        public void MarkingTurnAsPlayedKeepsRotatingTheTurn()
        {
            _turnSequence.Lead = Seat.South;

            var turn1 = _turnSequence.NextTurn();

            Assert.That(turn1, Is.Not.Null);

            turn1.MarkPlayed();

            var turn2 = _turnSequence.NextTurn();

            Assert.That(turn2, Is.Not.Null);
            Assert.That(turn2.Seat, Is.EqualTo(Seat.West));

            turn2.MarkPlayed();

            var turn3 = _turnSequence.NextTurn();

            Assert.That(turn3, Is.Not.Null);
            Assert.That(turn3.Seat, Is.EqualTo(Seat.North));

            turn3.MarkPlayed();

            var turn4 = _turnSequence.NextTurn();

            Assert.That(turn4, Is.Not.Null);
            Assert.That(turn4.Seat, Is.EqualTo(Seat.East));

            turn4.MarkPlayed();

            var turn5 = _turnSequence.NextTurn();

            Assert.That(turn5, Is.Not.Null);
            Assert.That(turn5.Seat, Is.EqualTo(Seat.South));
        }

        [Test]
        public void MarkingTurnRaisesTurnChanged()
        {
            var eventRaised = false;
            ITurn callbackTurn = null;

            _turnSequence.TurnChanged += (sender, args) =>
            {
                eventRaised = true;
                callbackTurn = args.Turn;
            };

            _turnSequence.Lead = Seat.South;

            var turn = _turnSequence.NextTurn();

            Assert.That(turn, Is.Not.Null);

            turn.MarkPlayed();

            var nxtTurn = _turnSequence.NextTurn();

            Assert.That(nxtTurn, Is.Not.Null);
            Assert.That(eventRaised, Is.True);
            Assert.That(callbackTurn, Is.Not.Null);
            Assert.That(nxtTurn.Seat, Is.EqualTo(callbackTurn.Seat));
        }

        [Test]
        public void SettingLeadRaisesTurnChanged()
        {
            var eventRaised = false;
            ITurn callbackTurn = null;

            _turnSequence.TurnChanged += (sender, args) =>
            {
                eventRaised = true;
                callbackTurn = args.Turn;
            };

            _turnSequence.Lead = Seat.South;

            Assert.That(eventRaised, Is.True);
            Assert.That(callbackTurn, Is.Not.Null);
            Assert.That(callbackTurn.Seat, Is.EqualTo(Seat.South));
        }

        [Test]
        public void AfterRestartLeadIsNull()
        {
            _turnSequence.Lead = Seat.North;

            _turnSequence.Restart();

            Assert.That(_turnSequence.Lead, Is.Null);
        }

        [Test]
        public void AfterRestartNextTurnIsNull()
        {
            _turnSequence.Lead = Seat.North;

            _turnSequence.Restart();

            Assert.That(_turnSequence.NextTurn(), Is.Null);
        }

        [Test]
        public void RestartRaisesRestartedEvent()
        {
            var eventRaised = false;
            _turnSequence.Restarted += (sender, args) => eventRaised = true;

            _turnSequence.Restart();

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void MarkPlayedAfterRestartDoesNotRaiseTurnChangedEvent()
        {
            _turnSequence.Lead = Seat.East;

            var nxtTurn = _turnSequence.NextTurn();

            Assert.That(nxtTurn, Is.Not.Null);

            var eventRaised = false;
            _turnSequence.TurnChanged += (sender, args) => eventRaised = true;

            _turnSequence.Restart();

            nxtTurn.MarkPlayed();

            Assert.Multiple(() =>
            {
                Assert.That(eventRaised, Is.False);
                Assert.That(_turnSequence.Lead, Is.Null);
                Assert.That(_turnSequence.NextTurn(), Is.Null);
            });
        }
    }
}