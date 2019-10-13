using Company.Client.Bets.Models;

namespace Company.Client.Bets.Interfaces.Observers
{
    interface IEventInfoObserver
    {
        void PopulateEventInfo(EventInfo[] items);
    }
}
