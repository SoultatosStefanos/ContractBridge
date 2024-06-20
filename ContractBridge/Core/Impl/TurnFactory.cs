namespace ContractBridge.Core.Impl
{
    public class TurnFactory : ITurnFactory
    {
        public ITurn Create(Seat seat)
        {
            return new Turn(seat);
        }
    }
}