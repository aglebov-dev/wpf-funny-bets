namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class DomainKindModel
    {
        public bool IsSelected { get; set; }
        public DomainKind? DomainKind { get; set; }
    }
}
