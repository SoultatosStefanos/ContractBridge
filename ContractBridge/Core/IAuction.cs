using System;
using System.Collections.Generic;

namespace ContractBridge.Core
{
    public interface IAuction
    {
        IBoard Board { get; }

        IEnumerable<IBid> AllBids { get; }

        IContract? FinalContract { get; }

        bool CanCall(IBid bid, ITurn turn);

        bool CanPass(ITurn turn);

        bool CanDouble(ITurn turn);

        bool CanRedouble(ITurn turn);

        void Call(IBid bid, ITurn turn);

        void Pass(ITurn turn);

        void Double(ITurn turn);

        void Redouble(ITurn turn);

        event EventHandler<CallEventArgs> Called;

        event EventHandler<PassEventArgs> Passed;

        event EventHandler<DoubleEventArgs> Doubled;

        event EventHandler<RedoubleEventArgs> Redoubled;

        event EventHandler<ContractEventArgs> FinalContractMade;

        public sealed class CallEventArgs : EventArgs
        {
            public CallEventArgs(IAuction auction, IBid bid, ITurn turn)
            {
                Auction = auction;
                Bid = bid;
                Turn = turn;
            }

            public IAuction Auction { get; }

            public IBid Bid { get; }

            public ITurn Turn { get; }
        }

        public sealed class PassEventArgs : EventArgs
        {
            public PassEventArgs(IAuction auction, ITurn turn)
            {
                Auction = auction;
                Turn = turn;
            }

            public IAuction Auction { get; }

            public ITurn Turn { get; }
        }

        public sealed class DoubleEventArgs : EventArgs
        {
            public DoubleEventArgs(IAuction auction, ITurn turn)
            {
                Auction = auction;
                Turn = turn;
            }

            public IAuction Auction { get; }

            public ITurn Turn { get; }
        }

        public sealed class RedoubleEventArgs : EventArgs
        {
            public RedoubleEventArgs(IAuction auction, ITurn turn)
            {
                Auction = auction;
                Turn = turn;
            }

            public IAuction Auction { get; }

            public ITurn Turn { get; }
        }

        public sealed class ContractEventArgs : EventArgs
        {
            public ContractEventArgs(IAuction auction, IContract contract)
            {
                Auction = auction;
                Contract = contract;
            }

            public IAuction Auction { get; }

            public IContract Contract { get; }
        }
    }
}