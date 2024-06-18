namespace ContractBridge.Core.Impl
{
    public class HandFactory : IHandFactory
    {
        public IHand Create()
        {
            return new Hand();
        }
    }
}