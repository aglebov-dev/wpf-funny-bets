using System;

namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class SuperexpressDrawInfoModel
    {
        public int Id { get; set; }
        public int SuperexpressId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartBetsDate { get; set; }
        public DateTime EndBetsDate { get; set; }
        public decimal MinBetAmount { get; set; }
        public decimal BaseAmount { get; set; }
        public int Number { get; set; }
        public int EventCount { get; set; }
        public bool IsSelected { get; set; }


        public string DrawCode => $"{Code}{Number}";
    }
}
