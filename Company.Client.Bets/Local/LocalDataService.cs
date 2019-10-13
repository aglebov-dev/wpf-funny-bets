using Company.Client.Bets.Interfaces;
using Company.Client.Bets.Interfaces.Observers;
using Company.Client.Bets.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Company.Client.Bets.Local
{
    class LocalDataService : IDataService
    {
        Storage Storage = new Storage();
        List<object> observers = new List<object>();
        private class DisposableObject : IDisposable
        {
            public void Dispose() { }
        }


        public BetInfoModel GetMadeBetInfo(int betID)
        {
            return Storage.Bets.FirstOrDefault(x => x.BetID == betID);
        }
        public IDisposable SubscribeOnEvent(int eventID, IEventObserver observer)
        {
            observer.PopulateEvent(
                Storage.Events.Select(x => new BetEventModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    StartTime = x.StartTime,
                    MaxAmount = x.MaxAmount,
                    Coefficients = new ObservableCollection<CoefficientModel>(x.Coefficients.Select(i => new CoefficientModel {
                        Id = i.Id,
                        Outcome = i.Outcome,
                        Value = i.Value
                    })),
                    TimeIntervals = new ObservableCollection<TimeIntervalModel>(x.TimeIntervals.Select(i => new TimeIntervalModel { Name = i.Name }))
                }).First(x => x.ID == eventID));
            observers.Add(observer);
            return new DisposableObject();
        }
        public IDisposable SubscribeOnEventsInfo(IEventInfoObserver observer)
        {
            observer.PopulateEventInfo(Storage.Events.Select(x => new EventInfo
            {
                ID = x.ID,
                DomainKind = x.DomainKind,
                IsIncludedInBet = false,
                IsLiveEvent = x.IsLiveEvent,
                Name = x.Name,
                StartTime = x.StartTime
            })
            .ToArray());
            observers.Add(observer);
            return new DisposableObject();
        }
        public IDisposable SubscribeOnMadeBets(IMadeBetsObserver observer)
        {
            observer.Populate(Storage.Bets
                .Select(x => new BetShortInfoModel
                {
                    BetDate = x.BetDate,
                    BetID = x.BetID,
                    BetState = x.BetResultState,
                    TotalBetAmount = x.BetAmount,
                    Content = new ObservableCollection<EventBetShortInfoModel>(x.BetEvents.Select(be => new EventBetShortInfoModel
                    {
                        EventName = be.Name,
                        IntervalName = be.Interval.Name,
                        OucomeName = be.Coefficient.Outcome.Name
                    }))
                }).ToArray());

            observers.Add(observer);
            return new DisposableObject();
        }
        public IDisposable SubscribeOnDraw(int id, ISuperexpressDrawObserver observer)
        {
            var draw = Storage.SuperexpressDraws
                .Select(x=> new SuperexpressDrawModel
                {
                    Id = x.Id,
                    SuperexpressId = x.SuperexpressId,
                    Name = x.Name,
                    EndBetsDateUtc = x.EndBetsDate,
                    StartBetsDateUtc = x.StartBetsDate,
                    Events = x.Events
                })
                .First(x => x.Id == id);
            observer.PopulateDraw(draw);
            observers.Add(observer);
            return new DisposableObject();
        }

        public IDisposable SubscribeOnDrawsInfo(ISuperexpressDrawsInfoObserver observer)
        {
            observer.Invalidate(Storage.SuperexpressDraws.Select(x => new SuperexpressDrawInfoModel
            {
                Id = x.Id,
                BaseAmount = x.BaseAmount,
                Code = x.Code,
                EndBetsDate = x.EndBetsDate,
                EventCount = x.EventCount,
                IsSelected = false,
                MinBetAmount = x.MinBetAmount,
                Name = x.Name,
                Number = x.Number,
                StartBetsDate = x.StartBetsDate,
                SuperexpressId = x.SuperexpressId
            }).ToArray());
            observers.Add(observer);
            return new DisposableObject();
        }
    }
}
