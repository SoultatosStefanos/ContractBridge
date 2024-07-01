using System;
using System.Collections.Generic;

namespace ContractBridge.Core
{
    public class AuctionPlayException : Exception
    {
    }

    public class AuctionPlayOutOfTurnException : AuctionPlayException
    {
    }

    public class AuctionCallTooLowException : AuctionPlayException
    {
    }

    public class AuctionDoubleOnPartnerException : AuctionPlayException
    {
    }

    public class AuctionDoubleBeforeCallException : AuctionPlayException
    {
    }

    public class AuctionReReDoubleException : AuctionPlayException
    {
    }

    public class AuctionFinalContractAlreadyMade : AuctionPlayException
    {
        
    }

    public interface IAuction
    {
        IContract? FinalContract { get; }

        IEnumerable<IBid> AllBids { get; }

        Seat? FirstTurn { get; set; }

        Seat? Turn { get; }

        bool CanCall(IBid bid, Seat seat);

        bool CanPass(Seat seat);

        bool CanDouble(Seat seat);

        void Call(IBid bid, Seat seat);

        void Pass(Seat seat);

        void Double(Seat seat);

        event EventHandler<TurnEventArgs> TurnChanged;

        event EventHandler<CallEventArgs> Called;

        event EventHandler<PassEventArgs> Passed;

        event EventHandler<DoubleEventArgs> Doubled;

        event EventHandler<RedoubleEventArgs> Redoubled;

        event EventHandler<ContractEventArgs> FinalContractMade;

        event EventHandler PassedOut;

        public sealed class TurnEventArgs : EventArgs
        {
            public TurnEventArgs(Seat seat)
            {
                Seat = seat;
            }

            public Seat Seat { get; }
        }

        public sealed class CallEventArgs : EventArgs
        {
            public CallEventArgs(IBid bid, Seat seat)
            {
                Bid = bid;
                Seat = seat;
            }

            public IBid Bid { get; }

            public Seat Seat { get; }
        }

        public sealed class PassEventArgs : EventArgs
        {
            public PassEventArgs(Seat seat)
            {
                Seat = seat;
            }

            public Seat Seat { get; }
        }

        public sealed class DoubleEventArgs : EventArgs
        {
            public DoubleEventArgs(Seat seat)
            {
                Seat = seat;
            }

            public Seat Seat { get; }
        }

        public sealed class RedoubleEventArgs : EventArgs
        {
            public RedoubleEventArgs(Seat seat)
            {
                Seat = seat;
            }

            public Seat Seat { get; }
        }

        public sealed class ContractEventArgs : EventArgs
        {
            public ContractEventArgs(IContract contract)
            {
                Contract = contract;
            }

            public IContract Contract { get; }
        }
    }
}