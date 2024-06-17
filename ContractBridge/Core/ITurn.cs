namespace ContractBridge.Core
{
    public interface ITurn
    {
        delegate void MarkPlayedHandler(ITurn turn);

        Seat Seat { get; }

        bool IsPlayed();

        void MarkPlayed();

        event MarkPlayedHandler MarkedPlayed;
    }
}