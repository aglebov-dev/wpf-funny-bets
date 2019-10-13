namespace Company.Client.Bets.Models
{
    class EventBetInfoModel: EventBaseModel
    {
        public TimeIntervalModel Interval { get; set; }
        public CoefficientModel Coefficient { get; set; }
        public bool? IsWin { get; set; }
    }
}
