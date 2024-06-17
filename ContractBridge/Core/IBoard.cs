using System;

namespace ContractBridge.Core
{
    public interface IBoard
    {
        delegate void DealerSetHandler(IBoard board, Seat seat);

        delegate void VulnerabilitySetHandler(IBoard board, Vulnerability vulnerability);

        Seat? Dealer { get; set; }

        Vulnerability? Vulnerability { get; set; }

        IPairFactory PairFactory { get; }

        IHandFactory HandFactory { get; }

        IPair Pair(Seat seat);

        IPair OtherPair(Seat seat);

        IPair OtherPair(IPair pair);

        IHand Hand(Seat seat);

        (IHand hand1, IHand hand2, IHand hand3) OtherHands(Seat seat);

        (IHand hand1, IHand hand2, IHand hand3) OtherHands(IHand hand);

        event DealerSetHandler DealerSet;

        event VulnerabilitySetHandler VulnerabilitySet;
    }

    public static class BoardExtensions
    {
        public static bool IsVulnerable(this IBoard board, Seat seat)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}