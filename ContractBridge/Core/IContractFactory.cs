namespace ContractBridge.Core
{
    public interface IContractFactory
    {
        IContract NewContract(byte level, Denomination denomination, Seat declarer, Risk risk);
    }
}