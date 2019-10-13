using System;
using System.Collections.ObjectModel;

namespace Company.Client.Bets.Models
{
    class BetInfoModel
    {
        public AccountInfoModel AccountInfo { get; set; }
        public int BetID { get; set; }
        public DateTime BetDate { get; set; }
        public string BetPointName { get; set; }
        public decimal BetAmount { get; set; }
        public decimal MightWinSum { get; set; }
        public decimal BonusAmount { get; set; }
        

        //в процессе/выигрыла/проиграла
        public BetResultState BetResultState { get; set; }
        public float? TotalCoefficient { get; set; }

        public decimal PayoutAmount { get; set; }

        //тип расчета наличные/безнал/бонусные баллы/акция
        public bool PaymentType { get; set; }


        public ObservableCollection<EventBetInfoModel> BetEvents { get; set; }
    }
}
