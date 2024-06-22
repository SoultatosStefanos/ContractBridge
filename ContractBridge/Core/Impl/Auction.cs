using System;
using System.Collections.Generic;
using System.Linq;

namespace ContractBridge.Core.Impl
{
    internal enum BidEntryType
    {
        Call,
        Double,
        Redouble
    }

    internal class BidEntry
    {
        private BidEntry(IBid? bid, Seat seat, BidEntryType type)
        {
            Bid = bid;
            Seat = seat;
            Type = type;
        }

        public IBid? Bid { get; }

        public Seat Seat { get; }

        public BidEntryType Type { get; }

        public bool IsCall => Type == BidEntryType.Call;

        public bool IsDouble => Type == BidEntryType.Double;

        public bool IsRedouble => Type == BidEntryType.Redouble;

        public static BidEntry Call(IBid bid, Seat seat)
        {
            return new BidEntry(bid, seat, BidEntryType.Call);
        }

        public static BidEntry Double(Seat seat)
        {
            return new BidEntry(null, seat, BidEntryType.Double);
        }

        public static BidEntry Redouble(Seat seat)
        {
            return new BidEntry(null, seat, BidEntryType.Redouble);
        }
    }

    public class Auction : IAuction
    {
        private readonly List<BidEntry> _bidsBySeat = new();

        private readonly IContractFactory _contractFactory;

        public Auction(IBoard board, IContractFactory contractFactory)
        {
            Board = board;
            _contractFactory = contractFactory;
        }

        public IBoard Board { get; }

        public IContract? FinalContract { get; }

        public bool CanCall(IBid bid, ITurn turn)
        {
            if (turn.IsPlayed())
            {
                return false;
            }

            if (LastBidSeat() is { } lastBidSeatValue)
            {
                if (lastBidSeatValue == turn.Seat)
                {
                    return false;
                }
            }

            if (LastBid() is { } lastBidValue)
            {
                if (bid.Level <= lastBidValue.Level && bid.Denomination <= lastBidValue.Denomination)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanPass(ITurn turn)
        {
            if (turn.IsPlayed())
            {
                return false;
            }

            if (LastBidSeat() is { } lastBidSeatValue)
            {
                if (lastBidSeatValue == turn.Seat)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanDouble(ITurn turn)
        {
            if (turn.IsPlayed())
            {
                return false;
            }

            if (LastBidSeat() is { } lastBidSeatValue)
            {
                if (lastBidSeatValue == turn.Seat)
                {
                    return false;
                }

                if (lastBidSeatValue == turn.Seat.Partner())
                {
                    return false;
                }
            }

            if (LastBidEntry() == null)
            {
                return false;
            }

            return true;
        }

        public void Call(IBid bid, ITurn turn)
        {
            if (turn.IsPlayed())
            {
                throw new AuctionTurnAlreadyPlayedException();
            }

            if (LastBidSeat() is { } lastBidSeatValue)
            {
                if (lastBidSeatValue == turn.Seat)
                {
                    throw new AuctionPlayAgainstSelfException();
                }
            }

            if (LastBid() is { } lastBidValue)
            {
                if (bid.Level <= lastBidValue.Level && bid.Denomination <= lastBidValue.Denomination)
                {
                    throw new AuctionCallTooLowException();
                }
            }

            _bidsBySeat.Add(BidEntry.Call(bid, turn.Seat));

            turn.MarkPlayed();
        }

        public void Pass(ITurn turn)
        {
            if (turn.IsPlayed())
            {
                throw new AuctionTurnAlreadyPlayedException();
            }

            if (LastBidSeat() is { } lastBidSeatValue)
            {
                if (lastBidSeatValue == turn.Seat)
                {
                    throw new AuctionPlayAgainstSelfException();
                }
            }

            turn.MarkPlayed();
        }

        public void Double(ITurn turn)
        {
            if (turn.IsPlayed())
            {
                throw new AuctionTurnAlreadyPlayedException();
            }

            if (LastBidSeat() is { } lastBidSeatValue)
            {
                if (lastBidSeatValue == turn.Seat)
                {
                    throw new AuctionPlayAgainstSelfException();
                }

                if (lastBidSeatValue == turn.Seat.Partner())
                {
                    throw new AuctionDoubleOnPartnerException();
                }
            }

            // TODO Double on nothing!

            if (LastBidEntry() is { } lastBidEntryValue)
            {
                _bidsBySeat.Add(MakeDoubleBidEntry(lastBidEntryValue, turn.Seat));

                turn.MarkPlayed();
            }
        }

        public event EventHandler<IAuction.CallEventArgs>? Called;
        public event EventHandler<IAuction.PassEventArgs>? Passed;
        public event EventHandler<IAuction.DoubleEventArgs>? Doubled;
        public event EventHandler<IAuction.RedoubleEventArgs>? Redoubled;
        public event EventHandler<IAuction.ContractEventArgs>? FinalContractMade;

        private static BidEntry MakeDoubleBidEntry(BidEntry lastBidEntry, Seat seat)
        {
            return lastBidEntry.Type switch
            {
                BidEntryType.Call => BidEntry.Double(seat),
                BidEntryType.Double => BidEntry.Redouble(seat),
                BidEntryType.Redouble => throw new AuctionReReDoubleException(),
                _ => null!
            };
        }

        private IBid? LastBid()
        {
            return LastBidEntry()?.Bid;
        }

        private Seat? LastBidSeat()
        {
            return LastBidEntry()?.Seat;
        }

        private BidEntry? LastBidEntry()
        {
            return _bidsBySeat.LastOrDefault();
        }
    }
}