using System;
using System.Collections.Generic;
using System.Linq;

namespace ContractBridge.Core.Impl
{
    internal class BidEntry
    {
        public BidEntry(IBid bid, Seat seat)
        {
            Bid = bid;
            Seat = seat;
        }

        public IBid Bid { get; }
        public Seat Seat { get; }

        public void Deconstruct(out IBid bid, out Seat seat)
        {
            bid = Bid;
            seat = Seat;
        }
    }

    public class Auction : IAuction
    {
        private readonly List<BidEntry> _bidEntries = new();

        private readonly IContractFactory _contractFactory;

        public Auction(IBoard board, IContractFactory contractFactory)
        {
            Board = board;
            _contractFactory = contractFactory;
        }

        public IBoard Board { get; }

        public IContract? FinalContract { get; }

        public IEnumerable<IBid> AllBids => _bidEntries.Select(entry => entry.Bid);

        public bool CanCall(IBid bid, ITurn turn)
        {
            if (turn.IsPlayed())
            {
                return false;
            }

            if (LastBidEntry() is var (lastBid, lastBidSeat))
            {
                if (lastBidSeat == turn.Seat)
                {
                    return false;
                }

                if (IsCallTooLow(bid, lastBid))
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

            if (LastBidEntry() is var (_, lastBidSeat))
            {
                if (lastBidSeat == turn.Seat)
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

            if (LastBidEntry() is var (lastBid, lastBidSeat))
            {
                if (lastBidSeat == turn.Seat)
                {
                    return lastBid.IsDoubled(); // Redouble.
                }

                if (lastBidSeat == turn.Seat.Partner())
                {
                    return lastBid.IsDoubled(); // Redouble.
                }

                if (lastBid.IsDoubled())
                {
                    return false;
                }
            }
            else
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

            if (LastBidEntry() is var (lastBid, lastBidSeat))
            {
                if (lastBidSeat == turn.Seat)
                {
                    throw new AuctionPlayAgainstSelfException();
                }

                if (IsCallTooLow(bid, lastBid))
                {
                    throw new AuctionCallTooLowException();
                }
            }

            _bidEntries.Add(new BidEntry(bid, turn.Seat));

            turn.MarkPlayed();
        }

        public void Pass(ITurn turn)
        {
            if (turn.IsPlayed())
            {
                throw new AuctionTurnAlreadyPlayedException();
            }

            if (LastBidEntry() is var (_, lastBidSeat))
            {
                if (lastBidSeat == turn.Seat)
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

            if (LastBidEntry() is var (lastBid, lastBidSeat))
            {
                if (lastBidSeat == turn.Seat)
                {
                    throw new AuctionPlayAgainstSelfException();
                }

                if (lastBidSeat == turn.Seat.Partner())
                {
                    if (!lastBid.IsDoubled()) // Else redouble
                    {
                        throw new AuctionDoubleOnPartnerException();
                    }
                }

                lastBid.Double();

                turn.MarkPlayed();
            }
            // TODO Double on nothing!
        }

        public event EventHandler<IAuction.CallEventArgs>? Called;
        public event EventHandler<IAuction.PassEventArgs>? Passed;
        public event EventHandler<IAuction.DoubleEventArgs>? Doubled;
        public event EventHandler<IAuction.RedoubleEventArgs>? Redoubled;
        public event EventHandler<IAuction.ContractEventArgs>? FinalContractMade;

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