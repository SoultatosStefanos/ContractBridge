namespace ContractBridge.Core
{
    public interface ICard
    {
        Rank Rank { get; }

        Suit Suit { get; }
    }
}