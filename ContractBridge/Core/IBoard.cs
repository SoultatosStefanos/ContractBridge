using System;
using System.Collections.Generic;

namespace ContractBridge.Core
{
    public class UnknownHandException : Exception
    {
    }

    public interface IBoard
    {
        Seat? Dealer { get; set; }

        Vulnerability? Vulnerability { get; set; }

        IEnumerable<IHand> Hands { get; }

        IHand Hand(Seat seat);

        IEnumerable<IHand> OtherHands(Seat seat);

        IEnumerable<IHand> OtherHands(IHand hand);

        event EventHandler<DealerEventArgs> DealerSet;

        event EventHandler<VulnerabilityEventArgs> VulnerabilitySet;

        public sealed class DealerEventArgs : EventArgs
        {
            public DealerEventArgs(Seat dealer)
            {
                Dealer = dealer;
            }

            public Seat Dealer { get; }
        }

        public sealed class VulnerabilityEventArgs : EventArgs
        {
            public VulnerabilityEventArgs(Vulnerability vulnerability)
            {
                Vulnerability = vulnerability;
            }

            public Vulnerability Vulnerability { get; }
        }
    }

    public static class BoardExtensions
    {
        public static bool IsVulnerable(this IBoard board, Seat seat)
        {
            var vulnerability = board.Vulnerability;

            if (vulnerability == Vulnerability.All)
            {
                return true;
            }

            return seat switch
            {
                Seat.East or Seat.West => vulnerability == Vulnerability.EastWest,
                Seat.North or Seat.South => vulnerability == Vulnerability.NorthSouth,
                _ => false
            };
        }
    }
}