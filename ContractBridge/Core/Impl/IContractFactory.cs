namespace ContractBridge.Core.Impl
{
    public interface IContractFactory
    {
        IContract Create(Level level, Denomination denomination, Seat declarer, Risk? risk);
    }
}