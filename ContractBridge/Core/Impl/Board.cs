using System;
using System.Collections.Generic;
using System.Text;

namespace ContractBridge.Core.Impl
{
    internal static class EnumerableExtensions
    {
        internal static IEnumerable<T> FromValues<T>(T value1, T value2, T value3, T value4)
        {
            yield return value1;
            yield return value2;
            yield return value3;
            yield return value4;
        }

        internal static IEnumerable<T> FromValues<T>(T value1, T value2, T value3)
        {
            yield return value1;
            yield return value2;
            yield return value3;
        }
    }

    public class Board : IBoard
    {
        private readonly IHand _eastHand;

        private readonly IHand _northHand;

        private readonly IHand _southHand;

        private readonly IHand _westHand;

        private Seat? _dealer;

        private Vulnerability? _vulnerability;

        public Board(IHandFactory handFactory)
        {
            _eastHand = handFactory.Create();
            _northHand = handFactory.Create();
            _southHand = handFactory.Create();
            _westHand = handFactory.Create();
        }

        public Seat? Dealer
        {
            get => _dealer;
            set
            {
                _dealer = value;

                if (_dealer is { } dealerValue)
                {
                    RaiseDealerSetEvent(dealerValue);
                }
            }
        }

        public Vulnerability? Vulnerability
        {
            get => _vulnerability;
            set
            {
                _vulnerability = value;

                if (_vulnerability is { } vulnerabilityValue)
                {
                    RaiseVulnerabilitySetEvent(vulnerabilityValue);
                }
            }
        }

        public IEnumerable<IHand> Hands => EnumerableExtensions.FromValues(
            _northHand,
            _southHand,
            _eastHand,
            _westHand
        );

        public IHand Hand(Seat seat)
        {
            return seat switch
            {
                Seat.East => _eastHand,
                Seat.North => _northHand,
                Seat.South => _southHand,
                Seat.West => _westHand,
                _ => throw new ArgumentOutOfRangeException(nameof(seat), seat, null)
            };
        }

        public IEnumerable<IHand> OtherHands(Seat seat)
        {
            var (h1, h2, h3) = GetOtherHands(seat);
            return EnumerableExtensions.FromValues(h1, h2, h3);
        }

        public IEnumerable<IHand> OtherHands(IHand hand)
        {
            if (ReferenceEquals(hand, _northHand)) return OtherHands(Seat.North);
            if (ReferenceEquals(hand, _southHand)) return OtherHands(Seat.South);
            if (ReferenceEquals(hand, _eastHand)) return OtherHands(Seat.East);
            if (ReferenceEquals(hand, _westHand)) return OtherHands(Seat.West);

            throw new UnknownHandException();
        }

        public event EventHandler<IBoard.DealerEventArgs>? DealerSet;

        public event EventHandler<IBoard.VulnerabilityEventArgs>? VulnerabilitySet;

        public string ToPbn()
        {
            if (Dealer is not { } dealer)
            {
                return "";
            }

            var seatCount = Enum.GetNames(typeof(Seat)).Length;

            var pbn = new StringBuilder();

            pbn.Append(dealer.ToString()[0]);
            pbn.Append(':');

            for (var i = 0; i < seatCount; ++i)
            {
                if (i != 0)
                {
                    pbn.Append(' ');
                }

                pbn.Append(Hand(dealer).ToPbn());

                dealer = dealer.NextSeat();
            }

            return pbn.ToString();
        }

        private bool Equals(Board other)
        {
            return _eastHand.Equals(other._eastHand)
                   && _northHand.Equals(other._northHand)
                   && _southHand.Equals(other._southHand)
                   && _westHand.Equals(other._westHand)
                   && _dealer == other._dealer
                   && _vulnerability == other._vulnerability;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Board)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _eastHand.GetHashCode();
                hashCode = (hashCode * 397) ^ _northHand.GetHashCode();
                hashCode = (hashCode * 397) ^ _southHand.GetHashCode();
                hashCode = (hashCode * 397) ^ _westHand.GetHashCode();
                return hashCode;
            }
        }

        private void RaiseDealerSetEvent(Seat dealerValue)
        {
            DealerSet?.Invoke(this, new IBoard.DealerEventArgs(dealerValue));
        }

        private void RaiseVulnerabilitySetEvent(Vulnerability vulnerabilityValue)
        {
            VulnerabilitySet?.Invoke(this, new IBoard.VulnerabilityEventArgs(vulnerabilityValue));
        }

        private (IHand h1, IHand h2, IHand h3) GetOtherHands(Seat seat)
        {
            return seat switch
            {
                Seat.East => (_northHand, _southHand, _westHand),
                Seat.North => (_eastHand, _southHand, _westHand),
                Seat.South => (_northHand, _eastHand, _westHand),
                Seat.West => (_northHand, _southHand, _eastHand),
                _ => throw new ArgumentOutOfRangeException(nameof(seat), seat, null)
            };
        }
    }
}