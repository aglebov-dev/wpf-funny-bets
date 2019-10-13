using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Company.Client.Bets.Models;
using Company.Client.Bets.ViewModels;

namespace Company.Client.Bets.Design
{
    class MadeBetDesign : MadeBetViewModel
    {
        internal static BetInfoModel betInfo;
        internal static BetInfoModel betInfo2;
        static Dictionary<int, string> IntervalNames;
        static Dictionary<int, string> OutcomeNames;
        static Dictionary<int, float> OutcomeValues;
        static Dictionary<int, string> EventNames;
        static MadeBetDesign()
        {
            IntervalNames = new Dictionary<int, string>
            {
                [0] = "Весь матч",
                [1] = "1-й тайм",
                [2] = "2-й тайм",
                [3] = "финальный сет"
            };
            OutcomeNames = new Dictionary<int, string>
            {
                [0] = "Победа второй команды",
                [1] = "X",
                [2] = "2",
                [3] = "1",
                [4] = "Победа первой команды",
                [5] = "Фора +5.5 счет первой команды"
            };
            OutcomeValues = new Dictionary<int, float>
            {
                [0] = 99.25f,
                [1] = 2.05f,
                [2] = 3.18f,
                [3] = 5.15f,
                [4] = 45.12f,
                [5] = 6.99f
            };
            EventNames = new Dictionary<int, string>
            {
                [0] = "Зениц - ЦСКА",
                [1] = "Локоматив - Спартак",
                [2] = "Терек - Анжи",
                [3] = "Тосно - Рубин",
                [4] = "Ростов Краснодар",
                [5] = "Сборная Англии - Сборная России"
            };
            betInfo = new BetInfoModel
            {
                AccountInfo = new AccountInfoModel
                {
                    ID = 123,
                    IsWoman = true,
                    Name = "Кира Найтли Анатольевна"
                },
                BetID = 170325879,
                BetDate = new DateTime(2017, 05, 16, 16, 25, 0),
                BetPointName = "Санкт-Петербург, ул. Востания 51",
                BetAmount = 100_000,
                PaymentType = true,
                TotalCoefficient = 35.16f,
                MightWinSum = 3_516_000,
                BonusAmount = 300_000_000,
                BetResultState = BetResultState.Win,
                PayoutAmount = 3_516_000,
                BetEvents = new ObservableCollection<EventBetInfoModel>(Enumerable.Range(150425, 20)
                .Select(x => new EventBetInfoModel
                {
                    ID = x,
                    Interval = new TimeIntervalModel
                    {
                        Name = IntervalNames[x % 4]
                    },
                    IsWin = x % 3 == 0,
                    Name = EventNames[x % 6],
                    StartTime = DateTime.Now,
                    Coefficient = new CoefficientModel
                    {
                        Id = x * 314,
                        Outcome = new OutcomeModel { Id = x , Name = OutcomeNames[x % 4] },
                        Value = OutcomeValues[x % 6]
                    }
                }))
            };
            betInfo2 = new BetInfoModel
            {
                AccountInfo = new AccountInfoModel
                {
                    ID = 777,
                    IsWoman = false,
                    Name = "Райан Гослинг Владимирович"
                },
                BetID = 170325879,
                BetDate = new DateTime(2017, 05, 16, 16, 25, 0),
                BetPointName = "На сайте",
                BetAmount = 6850,
                PaymentType = false,
                TotalCoefficient = 12.95f,
                MightWinSum = 72_265,
                BonusAmount = 2600,
                BetResultState = BetResultState.Lose,
                PayoutAmount = 0,
                BetEvents = new ObservableCollection<EventBetInfoModel>(Enumerable.Range(150425, 6)
                .Select(x => new EventBetInfoModel
                {
                    ID = x,
                    Interval = new TimeIntervalModel
                    {
                        Name = IntervalNames[(x * 2) % 4]
                    },
                    IsWin = (x * x) % 3 == 0,
                    Name = EventNames[(x + 12) % 6],
                    StartTime = DateTime.Now,
                    Coefficient = new CoefficientModel
                    {
                        Id = x * 314,
                        Outcome = new OutcomeModel { Id = x, Name = OutcomeNames[(x * 3) % 4] },
                        Value = OutcomeValues[(x * 7) % 6]
                    }
                }))
            };

        }


        public MadeBetDesign() : base(null, betInfo) { }


    }
}
