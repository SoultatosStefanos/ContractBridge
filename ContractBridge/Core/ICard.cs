namespace ContractBridge.Core
{
    public interface ICard : IPbnSerializable
    {
        Rank Rank { get; }

        Suit Suit { get; }
    }
}