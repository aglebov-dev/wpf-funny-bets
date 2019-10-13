using Company.Client.Bets.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Company.Client.Bets.Local
{
    class Storage
    {
        private const int OUTCOME_LENGHT = 25;
        private const int DOMAIN_KIND_POPULATION = 12;

        Dictionary<int, OutcomeModel> Outcomes = Enumerable.Range(0, OUTCOME_LENGHT)
            .ToDictionary(x => x, x => new OutcomeModel { Id = x * 1000, Name = $"Outcome{x * 1000}" });

        public List<EventLocal> Events { get; set; }
        public List<BetInfoModel> Bets { get; set; }
        public List<SuperexpressDrawLocal> SuperexpressDraws { get; set; }

        public Storage()
        {
            var liveRandom = new Random(7410005);
            var outcomeValueRandom = new Random(3658941);
            Events = Enumerable.Range(1, 100)
                .Select(x => new EventLocal
                {
                    ID = x * 12345,
                    DomainKind = (DomainKind)(x % DOMAIN_KIND_POPULATION),
                    MaxAmount = 12000,
                    Name = $"Сборная {x} - Не {x} сборная",
                    StartTime = StaticData.EventStartDates[(x % 3)].AddDays(x * 8).AddHours(-x * 13).AddMinutes(x * 10),
                    TimeIntervals = new ObservableCollection<TimeIntervalLocal> { new TimeIntervalLocal { Id = 1, Name = "Весь матч" } },
                    Coefficients = new ObservableCollection<CoefficientLocal>(CreateOutcomes(x, outcomeValueRandom)),
                    IsLiveEvent = liveRandom.Next(1, 3) % 2 == 0
                })
                .ToList();

            var random = new Random(38754);
            Bets = (from x in Enumerable.Range(1, 12)

                    let events = from e in Enumerable.Range(1, random.Next(1, 10))
                            let ev = Events[random.Next(1, 100)]
                            let outcome = ev.Coefficients[random.Next(0, 49)]
                            let interval = new TimeIntervalModel { Name = ev.TimeIntervals.First().Name }
                            select new EventBetInfoModel
                            {
                                ID = ev.ID,
                                Interval = interval,
                                IsWin = ev.ID % 4 == 0,
                                Name = ev.Name,
                                StartTime = ev.StartTime,
                                Coefficient = new CoefficientModel {
                                    Id = outcome.Id,
                                    Outcome = Outcomes[outcome.Id % OUTCOME_LENGHT],
                                    Value = outcome.Value
                                }
                            }
                    let betEvents = new ObservableCollection<EventBetInfoModel>(events)
                    let amount = x * 123
                    let totalCoefficient = (decimal)betEvents.Aggregate(1f, (acc, i) => acc * i.Coefficient.Value)
                    select new BetInfoModel
                    {
                        AccountInfo = new AccountInfoModel
                        {
                            ID = x * 345,
                            IsWoman = x % 2 == 0,
                            Name = x % 2 == 0 ? "Василиса Прекрасная" : "Иван Муромец"
                        },
                        BetAmount = amount,
                        BetDate = StaticData.EventStartDates[(x % 3)].AddDays(-x * 3).AddHours(-x * 13).AddMinutes(x * 10),
                        BetEvents = betEvents,
                        BetID = x * 987,
                        BetPointName = $"ППС {x}",
                        BetResultState = x % 3 == 0 ? BetResultState.Unknown : x % 2 == 0 ? BetResultState.Lose : BetResultState.Win,
                        BonusAmount = amount * 10,
                        MightWinSum = amount * totalCoefficient,
                        PaymentType = x % 2 == 0,
                        PayoutAmount = (x % 3 == 0 ? amount : x % 2 == 0.0m ? amount * 0.8m : 0.0m),
                        TotalCoefficient = (float)totalCoefficient

                    }).ToList();

            var seSelectorRandom = new Random(85452147);
            SuperexpressDraws = (from x in Enumerable.Range(1, 25)
                                 let outcomes = Enumerable.Range(0, seSelectorRandom.Next(3, 6)).Select(_ => Outcomes[seSelectorRandom.Next(0, OUTCOME_LENGHT)].Id).ToArray()
                                 let events = from e in Enumerable.Range(1, seSelectorRandom.Next(1, 25))
                                              let ev = Events[seSelectorRandom.Next(1, 100)]
                                              let coefficients = ev.Coefficients.Where(x=> outcomes.Contains(x.Outcome.Id))
                                              select new SuperexpressEventModel
                                              {
                                                  ID = ev.ID,
                                                  Name = ev.Name,
                                                  StartTime = ev.StartTime,
                                                  Coefficients = coefficients.Select(xxx => new CoefficientModel { Id = xxx.Id, Outcome = xxx.Outcome, Value = xxx.Value }).ToArray()
                                              }
                                 select new SuperexpressDrawLocal
                                 {
                                     Id = x * 654,
                                     SuperexpressId = x * 745 - 254,
                                     BaseAmount = x * 25 + 30,
                                     MinBetAmount = 100,
                                     Code = $"FDKP",
                                     StartBetsDate = StaticData.EventStartDates[(x % 3)].AddDays(x * 2).AddHours(x * 3).AddMinutes(x * 4),
                                     EndBetsDate = StaticData.EventStartDates[(x % 3)].AddDays(-x * 3).AddHours(x * 4).AddMinutes(x * 5),
                                     Name = $"Тираж суперэкспресса {x}",
                                     Number = x,
                                     Events = events.ToArray()
                                 }).ToList();
        }

        private CoefficientLocal[] CreateOutcomes(int eventID, Random outcomeValueRandom)
        {
            return (from i in Enumerable.Range(1, 50)
                    let id = int.Parse($"{eventID}{i + 91}")
                    select new CoefficientLocal {
                        Id = id,
                        Outcome = Outcomes[id % OUTCOME_LENGHT],
                        Value = (float)(1 + (eventID + 1) * outcomeValueRandom.NextDouble() % 4)
                    }).ToArray();
        }
    }
    class StaticData
    {
        public static Dictionary<int, DateTime> EventStartDates = new Dictionary<int, DateTime>
        {
            [0] = new DateTime(2018, 09, 21, 18, 00, 00),
            [1] = new DateTime(2018, 02, 9, 22, 45, 00),
            [2] = new DateTime(2018, 11, 18, 13, 00, 00)
        };
    }
}
