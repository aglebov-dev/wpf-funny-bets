using Company.Client.Bets.Models;
using System;
using System.Collections.ObjectModel;

namespace Company.Client.Bets.Design
{
    class EventsDesign
    {
        public ObservableCollection<EventInfo> Events { get; set; } = new ObservableCollection<EventInfo>();
        public ObservableCollection<DomainKindModel> DomainKinds { get; set; } = new ObservableCollection<DomainKindModel>();

        public EventsDesign()
        {

            for (int number = 0; number < 100; number++)
            {
                Events.Add(new EventInfo
                {
                    DomainKind = (DomainKind)(number % 10),
                    ID = number * 4781,
                    Name = $"Event number {number}",
                    StartTime = DateTime.Now,
                    IsIncludedInBet = number % 3 == 0
                });
            }

            for (int i = 0; i < 10; i++)
            {
                DomainKinds.Add(new DomainKindModel
                {
                    DomainKind = (DomainKind)i,
                    IsSelected = i % 3 == 0
                });
            }
        }
    }
}
