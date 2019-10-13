using Company.Client.Bets.Models;

namespace Company.Client.Bets.Interfaces.Observers
{
    interface IEventObserver
    {
        void PopulateEvent(BetEventModel eventModel);
        //void UpdateOutcomes(object data);
    }
}
