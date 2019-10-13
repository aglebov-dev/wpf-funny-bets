using Company.Client.Bets.Models;
namespace Company.Client.Bets.Interfaces.Observers
{
    interface ISuperexpressDrawObserver
    {
        void PopulateDraw(SuperexpressDrawModel model);
    }
}
