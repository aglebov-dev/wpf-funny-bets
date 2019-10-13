namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    abstract class BaseBetModel: BaseValidatedDataModel<BaseBetModel>
    {
        public int? BetID { get; set; }
        public decimal TotalBetAmount { get; set; }
    }
}
