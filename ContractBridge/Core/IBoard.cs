using System;

namespace ContractBridge.Core
{
    public interface IBoard
    {
        Seat? Dealer { get; set; }

        Vulnerability? Vulnerability { get; set; }

        IPair Pair(Seat seat);

        IPair OtherPair(Seat seat);

        IPair OtherPair(IPair pair);

        IHand Hand(Seat seat);

        (IHand hand1, IHand hand2, IHand hand3) OtherHands(Seat seat);

        (IHand hand1, IHand hand2, IHand hand3) OtherHands(IHand hand);

        event EventHandler<DealerEventArgs> DealerSet;

        event EventHandler<VulnerabilityEventArgs> VulnerabilitySet;

        public sealed class DealerEventArgs : EventArgs
        {
            public DealerEventArgs(IBoard board, Seat dealer)
            {
                Board = board;
                Dealer = dealer;
            }

            public IBoard Board { get; }

            public Seat Dealer { get; }
        }

        public sealed class VulnerabilityEventArgs : EventArgs
        {
            public VulnerabilityEventArgs(IBoard board, Vulnerability vulnerability)
            {
                Board = board;
                Vulnerability = vulnerability;
            }

            public IBoard Board { get; }

            public Vulnerability Vulnerability { get; }
        }
    }

    public static class BoardExtensions
    {
        public static bool IsVulnerable(this IBoard board, Seat seat)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}