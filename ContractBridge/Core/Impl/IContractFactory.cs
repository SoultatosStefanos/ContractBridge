namespace ContractBridge.Core
{
    public interface IContractFactory
    {
        IContract Create(Level level, Denomination denomination, Seat declarer, Risk? risk);
    }
}