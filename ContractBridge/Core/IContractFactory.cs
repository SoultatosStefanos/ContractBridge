namespace ContractBridge.Core
{
    public interface IContractFactory
    {
        IContract NewContract(Level level, Denomination denomination, Seat declarer, Risk? risk);
    }
}