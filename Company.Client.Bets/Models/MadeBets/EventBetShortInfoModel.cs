namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class EventBetShortInfoModel
    {
        public string EventName { get; set; }
        public string IntervalName { get; set; }
        public string OucomeName { get; set; }
    }
}
