namespace Company.Client.Bets.Models
{
    /// <summary>
    /// Информация о событии
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class EventInfo : EventBaseModel
    {
        public bool IsIncludedInBet { get; set; }
        public DomainKind DomainKind { get; set; }
        public bool IsLiveEvent { get; internal set; }
    }
}
