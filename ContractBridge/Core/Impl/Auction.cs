using System;
using System.Collections.Generic;
using System.Linq;

namespace ContractBridge.Core.Impl
{
    internal enum BidEntryState
    {
        Normal,
        Doubled,
        Redoubled
    }

    internal class BidEntry
    {
        public BidEntry(IBid bid, Seat seat)
        {
            Bid = bid;
            Seat = seat;
            State = BidEntryState.Normal;
        }

        public IBid Bid { get; }
        public Seat Seat { get; }

        public Seat? DoubledSeat { get; private set; }

        private BidEntryState State { get; set; }

        public bool IsDoubled()
        {
            return State == BidEntryState.Doubled;
        }

        public bool IsRedoubled()
        {
            return State == BidEntryState.Redoubled;
        }

        public void Double(Seat fromSeat)
        {
            switch (State)
            {
                case BidEntryState.Normal:
                    State = BidEntryState.Doubled;
                    DoubledSeat = fromSeat;
                    break;

                case BidEntryState.Doubled:
                    State = BidEntryState.Redoubled;
                    DoubledSeat = fromSeat;
                    break;

                case BidEntryState.Redoubled:
                    throw new AuctionReReDoubleException();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Deconstruct(out IBid bid, out Seat seat, out Seat? doubledSeat)
        {
            bid = Bid;
            seat = Seat;
            doubledSeat = DoubledSeat;
        }
    }

    public class Auction : IAuction
    {
        private readonly List<BidEntry> _bidEntries = new();

        private readonly IContractFactory _contractFactory;

        private readonly int _requiredPassCountToAdvance = Enum.GetNames(typeof(Seat)).Length - 1;

        private int _passCount;

        public Auction(ITurnPlayContext turnPlayContext, IContractFactory contractFactory)
        {
            TurnPlayContext = turnPlayContext;
            _contractFactory = contractFactory;
        }

        public IContract? FinalContract { get; private set; }

        public IEnumerable<IBid> AllBids => _bidEntries.Select(entry => entry.Bid);

        public ITurnPlayContext TurnPlayContext { get; }

        public bool CanCall(IBid bid, Seat seat)
        {
            return TurnPlayContext.CanPlayTurn(seat, () =>
            {
                if (LastBidEntry() is not var (lastBid, _, _)) // Entry call.
                {
                    return true;
                }

                return !IsCallTooLow(bid, lastBid);
            });
        }

        public bool CanPass(Seat seat)
        {
            return TurnPlayContext.CanPlayTurn(seat, () => true);
        }

        public bool CanDouble(Seat seat)
        {
            return TurnPlayContext.CanPlayTurn(seat, () =>
            {
                if (LastBidEntry() is not { } lastBidEntry)
                {
                    return false;
                }

                var (_, lastBidSeat, _) = lastBidEntry;

                if (lastBidEntry.IsRedoubled())
                {
                    return false;
                }

                if (lastBidSeat.Partnership() == seat.Partnership())
                {
                    return lastBidEntry.IsDoubled(); // Redouble chance.
                }

                return !lastBidEntry.IsDoubled();
            });
        }

        public void Call(IBid bid, Seat seat)
        {
            TurnPlayContext.PlayTurn(seat, () =>
            {
                if (LastBidEntry() is var (lastBid, _, _))
                {
                    if (IsCallTooLow(bid, lastBid))
                    {
                        throw new AuctionCallTooLowException();
                    }
                }

                _bidEntries.Add(new BidEntry(bid, seat));

                RaiseCalledEvent(bid, seat);

                _passCount = 0;
            });
        }

        public void Pass(Seat seat)
        {
            TurnPlayContext.PlayTurn(seat, () =>
            {
                RaisePassedEvent(seat);

                if (LastBidEntry() is { } lastBidEntry)
                {
                    if (!SavePassAndCheckForAdvance())
                    {
                        return;
                    }

                    FinalContract = MakeFinalContract(lastBidEntry);
                    RaiseFinalContractMade(FinalContract);
                }
                else
                {
                    if (SavePassAndCheckForAdvance())
                    {
                        RaisePassedOutEvent();
                    }
                }
            });
        }

        public void Double(Seat seat)
        {
            TurnPlayContext.PlayTurn(seat, () =>
            {
                if (LastBidEntry() is not { } lastBidEntry)
                {
                    throw new AuctionDoubleBeforeCallException();
                }

                var (_, lastBidSeat, _) = lastBidEntry;

                if (lastBidSeat.Partnership() == seat.Partnership())
                {
                    if (lastBidEntry.IsRedoubled())
                    {
                        throw new AuctionReReDoubleException();
                    }

                    if (!lastBidEntry.IsDoubled())
                    {
                        throw new AuctionDoubleOnPartnerException();
                    }

                    lastBidEntry.Double(seat);
                    RaiseRedoubledEvent(seat);
                }
                else
                {
                    lastBidEntry.Double(seat);
                    RaiseDoubledEvent(seat);
                }

                _passCount = 0;
            });
        }

        public event EventHandler<IAuction.CallEventArgs>? Called;
        public event EventHandler<IAuction.PassEventArgs>? Passed;
        public event EventHandler<IAuction.DoubleEventArgs>? Doubled;
        public event EventHandler<IAuction.RedoubleEventArgs>? Redoubled;
        public event EventHandler<IAuction.ContractEventArgs>? FinalContractMade;
        public event EventHandler? PassedOut;

        private bool SavePassAndCheckForAdvance()
        {
            return ++_passCount == _requiredPassCountToAdvance;
        }

        private IContract MakeFinalContract(BidEntry bidEntry)
        {
            var declarer = bidEntry.IsDoubled() || bidEntry.IsRedoubled()
                ? bidEntry.DoubledSeat!.Value
                : bidEntry.Seat;

            var risk = bidEntry.IsDoubled()
                ? Risk.Doubled
                : bidEntry.IsRedoubled()
                    ? Risk.Redoubled
                    : (Risk?)null;

            return _contractFactory.Create(bidEntry.Bid.Level, bidEntry.Bid.Denomination, declarer, risk);
        }

        private void RaiseFinalContractMade(IContract finalContract)
        {
            FinalContractMade?.Invoke(this, new IAuction.ContractEventArgs(finalContract));
        }

        private void RaiseCalledEvent(IBid bid, Seat seat)
        {
            Called?.Invoke(this, new IAuction.CallEventArgs(bid, seat));
        }

        private void RaisePassedEvent(Seat seat)
        {
            Passed?.Invoke(this, new IAuction.PassEventArgs(seat));
        }

        private void RaiseRedoubledEvent(Seat seat)
        {
            Redoubled?.Invoke(this, new IAuction.RedoubleEventArgs(seat));
        }

        private void RaiseDoubledEvent(Seat seat)
        {
            Doubled?.Invoke(this, new IAuction.DoubleEventArgs(seat));
        }

        private void RaisePassedOutEvent()
        {
            PassedOut?.Invoke(this, EventArgs.Empty);
        }

        private static bool IsCallTooLow(IBid bid, IBid lastBid)
        {
            return bid.Level <= lastBid.Level && bid.Denomination <= lastBid.Denomination;
        }

        private BidEntry? LastBidEntry()
        {
            return _bidEntries.LastOrDefault();
        }
    }
}