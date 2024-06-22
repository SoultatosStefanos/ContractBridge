using System;
using System.Collections.Generic;

namespace ContractBridge.Core
{
    public class InvalidAuctionPlayException : Exception
    {
    }

    public class AuctionTurnAlreadyPlayedException : InvalidAuctionPlayException
    {
    }

    public class AuctionPlayAgainstSelfException : InvalidAuctionPlayException
    {
    }

    public class AuctionCallTooLowException : InvalidAuctionPlayException
    {
    }

    public class AuctionDoubleOnPartnerException : InvalidAuctionPlayException
    {
    }

    public class AuctionDoubleBeforeCallException : InvalidAuctionPlayException
    {
    }

    public interface IAuction
    {
        IBoard Board { get; }

        IContract? FinalContract { get; }

        IEnumerable<IBid> AllBids { get; }

        bool CanCall(IBid bid, ITurn turn);

        bool CanPass(ITurn turn);

        bool CanDouble(ITurn turn);

        void Call(IBid bid, ITurn turn);

        void Pass(ITurn turn);

        void Double(ITurn turn);

        event EventHandler<CallEventArgs> Called;

        event EventHandler<PassEventArgs> Passed;

        event EventHandler<DoubleEventArgs> Doubled;

        event EventHandler<RedoubleEventArgs> Redoubled;

        event EventHandler<ContractEventArgs> FinalContractMade;

        event EventHandler PassedOut;

        public sealed class CallEventArgs : EventArgs
        {
            public CallEventArgs(IBid bid, ITurn turn)
            {
                Bid = bid;
                Turn = turn;
            }

            public IBid Bid { get; }

            public ITurn Turn { get; }
        }

        public sealed class PassEventArgs : EventArgs
        {
            public PassEventArgs(ITurn turn)
            {
                Turn = turn;
            }

            public ITurn Turn { get; }
        }

        public sealed class DoubleEventArgs : EventArgs
        {
            public DoubleEventArgs(ITurn turn)
            {
                Turn = turn;
            }

            public ITurn Turn { get; }
        }

        public sealed class RedoubleEventArgs : EventArgs
        {
            public RedoubleEventArgs(ITurn turn)
            {
                Turn = turn;
            }

            public ITurn Turn { get; }
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