using System.Collections.Generic;

namespace ContractBridge.Core
{
    public interface IAuction
    {
        delegate void CalledHandler(IAuction auction, IBid bid, ITurn turn);

        delegate void DoubledHandler(IAuction auction, ITurn turn);

        delegate void FinalContractMadeHandler(IAuction auction, IContract contract);

        delegate void PassedHandler(IAuction auction, ITurn turn);

        delegate void RedoubledHandler(IAuction auction, ITurn turn);

        IBoard Board { get; }

        IEnumerable<IBid> AllBids { get; }

        IContract FinalContract { get; }

        IContractFactory ContractFactory { get; }

        bool CanCall(IBid bid, ITurn turn);

        bool CanPass(ITurn turn);

        bool CanDouble(ITurn turn);

        bool CanRedouble(ITurn turn);

        void Call(IBid bid, ITurn turn);

        void Pass(ITurn turn);

        void Double(ITurn turn);

        void Redouble(ITurn turn);

        event CalledHandler Called;

        event PassedHandler Passed;

        event DoubledHandler Doubled;

        event RedoubledHandler Redoubled;

        event FinalContractMadeHandler FinalContractMade;
    }
}