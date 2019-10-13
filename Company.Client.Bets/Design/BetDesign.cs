using Company.Client.Bets.Models;
using Company.Client.Bets.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Company.Client.Bets.Design
{
    internal class BetDesign: BetViewModel
    {
        public BetDesign() : base(null, null, null, null)
        {
            Fill();
        }

        public void Fill()
        {
            var intervals = Enumerable.Range(1, 30)
                .Select(x => new TimeIntervalModel { Name = $"интервал {x}" })
                .ToArray();
            var outcomes = Enumerable.Range(1, 30)
                .Select(x => new CoefficientModel {
                    Id = x,
                    Value = x * 2.143f - x * 1.647f,
                    Outcome = new OutcomeModel { Id = x *125, Name = $"Исход {x}" }
                })
                .ToArray();

            for (int i = 0; i < 9; i++)
            {
                Bet.PopulateEvent(new BetEventModel
                {
                    ID = i *8954,
                    MaxAmount = i * 52000,
                    Name = $"Event {i}",
                    Coefficients = new ObservableCollection<CoefficientModel>(outcomes),
                    TimeIntervals = new ObservableCollection<TimeIntervalModel>(intervals),
                    StartTime = new DateTime(2018,09,17).AddDays(i).AddHours(-i).AddMinutes(i * 17),
                    SelectedTimeInterval = i % 3 == 0 ? null : intervals[i],
                    SelectedCoefficient = i % 3 == 0 ? null : outcomes[i]
                });
            }
        }
    }
}
