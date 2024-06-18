using ContractBridge.Core;
using NUnit.Framework;

namespace ContractBridge.Tests.Core
{
    [TestFixture]
    [TestOf(typeof(SeatExtensions))]
    public class SeatExtensionsTest
    {
        [Test]
        public void NextSeatFromNorthIsEast()
        {
            Assert.That(Seat.North.NextSeat(), Is.EqualTo(Seat.East));
        }

        [Test]
        public void NextSeatFromEastIsSouth()
        {
            Assert.That(Seat.East.NextSeat(), Is.EqualTo(Seat.South));
        }

        [Test]
        public void NextSeatFromSouthIsWest()
        {
            Assert.That(Seat.South.NextSeat(), Is.EqualTo(Seat.West));
        }

        [Test]
        public void NextSeatFromWestIsNorth()
        {
            Assert.That(Seat.West.NextSeat(), Is.EqualTo(Seat.North));
        }

        [Test]
        public void PartnerOfNorthIsSouth()
        {
            Assert.That(Seat.North.Partner(), Is.EqualTo(Seat.South));
        }

        [Test]
        public void PartnerOfSouthIsNorth()
        {
            Assert.That(Seat.South.Partner(), Is.EqualTo(Seat.North));
        }

        [Test]
        public void PartnerOfEastIsWest()
        {
            Assert.That(Seat.East.Partner(), Is.EqualTo(Seat.West));
        }

        [Test]
        public void PartnerOfWestIsEast()
        {
            Assert.That(Seat.West.Partner(), Is.EqualTo(Seat.East));
        }
    }
}