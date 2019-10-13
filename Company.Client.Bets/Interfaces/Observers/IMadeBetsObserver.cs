using Company.Client.Bets.Models;

namespace Company.Client.Bets.Interfaces.Observers
{
    interface IMadeBetsObserver
    {
        void Populate(BetShortInfoModel[] items);
    }
}
