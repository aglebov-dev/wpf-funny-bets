namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class SuperexpressBetModel: BaseBetModel
    {
        public long[] Coefficients { get; set; }
    }
}
