namespace ContractBridge.Core
{
    public interface ITurnManager
    {
        delegate void LeadSetHandler(ITurnManager manager, Seat lead);

        delegate void RestartHandler(ITurnManager manager);

        delegate void TurnChangeHandler(ITurnManager manager, ITurn turn);

        Seat? Lead { get; set; }

        ITurnFactory TurnFactory { get; }

        ITurn? NextTurn();

        void Restart();

        event LeadSetHandler LeadSet;

        event TurnChangeHandler TurnChanged;

        event RestartHandler Restarted;
    }
}