using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private Seat? _firstTurn;

        private int _passCount;

        private Seat? _turn;

        public Auction(IContractFactory contractFactory)
        {
            _contractFactory = contractFactory;
        }

        public IContract? FinalContract { get; private set; }

        public IEnumerable<IBid> AllBids => _bidEntries.Select(entry => entry.Bid);

        public Seat? FirstTurn
        {
            get => _firstTurn;
            set
            {
                _firstTurn = value;
                if (_firstTurn is { } firstTurn)
                {
                    Turn = firstTurn;
                }
            }
        }

        public Seat? Turn
        {
            get => _turn;
            private set
            {
                _turn = value;
                Debug.Assert(_turn != null, nameof(_turn) + " != null");
                RaiseTurnChangedEvent(_turn!.Value);
            }
        }

        public bool CanCall(IBid bid, Seat seat)
        {
            if (Turn is not { } turn)
            {
                return false;
            }

            if (FinalContract != null)
            {
                return false;
            }

            if (turn != seat)
            {
                return false;
            }

            if (LastBidEntry() is not var (lastBid, _, _)) // Entry call.
            {
                return true;
            }

            return !IsCallTooLow(bid, lastBid);
        }

        public bool CanPass(Seat seat)
        {
            if (Turn is not { } turn)
            {
                return false;
            }
            
            if (FinalContract != null)
            {
                return false;
            }

            return turn == seat;
        }

        public bool CanDouble(Seat seat)
        {
            if (Turn is not { } turn)
            {
                return false;
            }
            
            if (FinalContract != null)
            {
                return false;
            }

            if (turn != seat)
            {
                return false;
            }

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
        }

        public void Call(IBid bid, Seat seat)
        {
            if (Turn is not { } turn)
            {
                throw new AuctionPlayOutOfTurnException();
            }

            if (FinalContract != null)
            {
                throw new AuctionFinalContractAlreadyMade();
            }
            
            if (turn != seat)
            {
                throw new AuctionPlayOutOfTurnException();
            }

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
            AdvanceTurn(turn);
        }

        public void Pass(Seat seat)
        {
            if (Turn is not { } turn)
            {
                throw new AuctionPlayOutOfTurnException();
            }
            
            if (FinalContract != null)
            {
                throw new AuctionFinalContractAlreadyMade();
            }

            if (turn != seat)
            {
                throw new AuctionPlayOutOfTurnException();
            }

            RaisePassedEvent(seat);
            AdvanceTurn(turn);

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
        }

        public void Double(Seat seat)
        {
            if (Turn is not { } turn)
            {
                throw new AuctionPlayOutOfTurnException();
            }
            
            if (FinalContract != null)
            {
                throw new AuctionFinalContractAlreadyMade();
            }

            if (turn != seat)
            {
                throw new AuctionPlayOutOfTurnException();
            }

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
                AdvanceTurn(turn);
            }
            else
            {
                lastBidEntry.Double(seat);
                RaiseDoubledEvent(seat);
                AdvanceTurn(turn);
            }

            _passCount = 0;
        }

        public event EventHandler<IAuction.TurnEventArgs>? TurnChanged;

        public event EventHandler<IAuction.CallEventArgs>? Called;

        public event EventHandler<IAuction.PassEventArgs>? Passed;

        public event EventHandler<IAuction.DoubleEventArgs>? Doubled;

        public event EventHandler<IAuction.RedoubleEventArgs>? Redoubled;

        public event EventHandler<IAuction.ContractEventArgs>? FinalContractMade;

        public event EventHandler? PassedOut;

        private void AdvanceTurn(Seat turn)
        {
            Turn = turn.NextSeat();
        }

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

        private void RaiseTurnChangedEvent(Seat turn)
        {
            TurnChanged?.Invoke(this, new IAuction.TurnEventArgs(turn));
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