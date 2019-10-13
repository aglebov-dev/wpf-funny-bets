using System;

namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class SuperexpressDrawModel
    {
        public int Id { get; set; }
        public int SuperexpressId { get; set; }
        public string Name { get; set; }
        public DateTime StartBetsDateUtc { get; set; }
        public DateTime EndBetsDateUtc { get; set; }
        public SuperexpressEventModel[] Events { get; set; }
    }
}
