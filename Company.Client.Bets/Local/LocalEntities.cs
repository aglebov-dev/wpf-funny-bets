using Company.Client.Bets.Models;
using System;
using System.Collections.ObjectModel;

namespace Company.Client.Bets.Local
{
    class EventLocal
    {
        public int ID { get; set; }
        public DomainKind DomainKind { get; set; }
        public decimal MaxAmount { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public ObservableCollection<CoefficientLocal> Coefficients { get; set; }
        public ObservableCollection<TimeIntervalLocal> TimeIntervals { get; set; }
        public bool IsLiveEvent { get; internal set; }
    }
    class CoefficientLocal
    {
        public int Id { get; set; }
        public float Value { get; internal set; }

        public OutcomeModel Outcome { get; set; }
    }
    class TimeIntervalLocal
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    class SuperexpressDrawLocal
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
        public SuperexpressEventModel[] Events { get; set; }
    }
}
