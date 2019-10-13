using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Company.Client.Bets.Models;
using Company.Client.Bets.Interfaces;
using Company.Client.Bets.Interfaces.Observers;

namespace Company.Client.Bets.Local
{
    class DataServiceStub : IDataService
    {
        private class DisposableObject : IDisposable { public void Dispose() { } }

        public IDisposable SubscribeOnEvent(int eventID, IEventObserver observer)
        {
            //observer.PopulateEvent(new EventModel
            //{
            //    ID = eventID,
            //    MaxAmount = DateTime.Now.Millisecond * 100,
            //    Name = "Event " + eventID,
            //    StartTime = DateTime.Now.AddHours(6),
            //    Coefficients = new ObservableCollection<CoefficientModel>(
            //            Enumerable.Range(1,10).Select(x=> new CoefficientModel {Id = x, Name = "Исход " + x *33, Value = x * 1.248f })
            //        ),
            //    TimeIntervals = new ObservableCollection<TimeIntervalModel>(
            //            Enumerable.Range(1, 10).Select(x => new TimeIntervalModel { Name = "Интервал" + x * 33 })
            //        )
            //});

            return new DisposableObject();
        }

        public IDisposable SubscribeOnEventsInfo(IEventInfoObserver observer)
        {
            var events = Enumerable.Range(1, 53 * 4)
                .Select(x => new EventInfo
                {
                    ID = x,
                    DomainKind = (DomainKind)(x % 53),
                    Name = EventNames[x % 6]
                })
                .ToArray();


            observer.PopulateEventInfo(events);
            return null;
        }

        public IDisposable SubscribeOnDrawsInfo(ISuperexpressDrawsInfoObserver observer)
        {
            var draws = Enumerable.Range(1, 8)
                .Select(i => new SuperexpressDrawInfoModel
                {
                    BaseAmount = 10 + i * 4,
                    Code = $"FD9J",
                    Id = i * 132 + 9541,
                    MinBetAmount = 1.5m * (10 + i * 4),
                    Name = $"Футбол. Лига Европы.",
                    Number = i + 7 * 81,
                    SuperexpressId = i + 8,
                    EventCount = i * 2 + 6,
                    StartBetsDate = new DateTime(2018, 09, 17).AddDays(i).AddHours(-i).AddMinutes(i * 17),
                    EndBetsDate = new DateTime(2018, 11, 17).AddDays(-i).AddHours(+i).AddMinutes(-i * 17),
                });
            observer.Invalidate(draws.ToArray());

            return null;
        }

        public IDisposable SubscribeOnDraw(int id, ISuperexpressDrawObserver observer)
        {
            //observer.PopulateDraw(new SuperexpressDrawModel
            //{
            //    Events = Enumerable.Range(1, 15).Select(x => new SuperexpressEventModel 
            //    {
            //        ID = (x + 1) * 1234,
            //        Outcomes = Enumerable.Range(1, 5).Select(c => new CoefficientModel { Id = c,  Name = "outcome " + c, Value = c * 1.25f }).ToArray(),
            //        Name = "Event " + x,
            //        StartTime = DateTime.Now,
            //        BranchPathName = "path/path/path"
            //    }).ToArray(),
            //    Name = "DRAW",
            //    SuperexpressId = id + 250,
            //    Id = id,
            //    StartBetsDateUtc = DateTime.Now,
            //    EndBetsDateUtc = DateTime.Now
            //});

            return null;
        }

        public IDisposable SubscribeOnMadeBets(IMadeBetsObserver observer)
        {
            var items = Enumerable.Range(1, 60).Select(i => new Models.BetShortInfoModel
            {
                BetID = i * 1000 + i * 6548 - i * 745,
                BetDate = DateTime.Now,
                BetState = null,
                TotalBetAmount = i * 124,
                Content = new ObservableCollection<Models.EventBetShortInfoModel>(Enumerable.Range(1, 6).Select(x => new EventBetShortInfoModel
                {
                    EventName = $"Event {x}",
                    IntervalName = $"interval",
                    OucomeName = $"outcome {x * x}"
                }))
            }).ToArray();

            observer.Populate(items);

            return new DisposableObject();
        }

        public BetInfoModel GetMadeBetInfo(int betID)
        {
            return betID % 2 == 0 ? Design.MadeBetDesign.betInfo : Design.MadeBetDesign.betInfo2;
        }

        Dictionary<int, string> EventNames = new Dictionary<int, string>
        {
            [0] = "Зениц - ЦСКА",
            [1] = "Локоматив - Спартак",
            [2] = "Терек - Анжи",
            [3] = "Тосно - Рубин",
            [4] = "Ростов Краснодар",
            [5] = "Сборная Англии - Сборная России"
        };
    }

}
