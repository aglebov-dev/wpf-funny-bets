namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class BetState
    {
        public bool IsBusy { get; set; }
        public bool IsLock { get; set; }
        public bool IsFailure { get; set; }
    }
}
