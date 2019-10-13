using System;

namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    abstract class EventBaseModel: BaseValidatedDataModel<EventBaseModel>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }

        public string Number => $"{ID % 10000:00000}";
    }
}
