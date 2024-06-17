namespace ContractBridge.Core
{
    public interface ITurnManager
    {
        delegate void ClearHandler(ITurnManager manager);

        delegate void LeadSetHandler(ITurnManager manager, Seat lead);

        delegate void TurnChangeHandler(ITurnManager manager, ITurn turn);

        Seat Lead { get; set; }

        ITurnFactory TurnFactory { get; }

        ITurn NextTurn();

        void Clear();

        event LeadSetHandler LeadSet;

        event TurnChangeHandler TurnChanged;

        event ClearHandler Cleared;
    }
}