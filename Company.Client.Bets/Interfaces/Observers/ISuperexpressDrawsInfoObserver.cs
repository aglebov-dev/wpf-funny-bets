using Company.Client.Bets.Models;

namespace Company.Client.Bets.Interfaces.Observers
{
    interface ISuperexpressDrawsInfoObserver
    {
        void Invalidate(SuperexpressDrawInfoModel[] draws);
        void AppendDraw(SuperexpressDrawInfoModel draw);
        void UpdateDraw(SuperexpressDrawInfoModel draw);
        void RemoveDraw(int drawID);
    }
}
