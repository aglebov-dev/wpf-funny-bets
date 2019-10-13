using Company.Client.Bets.ViewModels;
using System;
using System.Linq;

namespace Company.Client.Bets.Design
{
    class MadeBetsDesign : MadeBetsViewModel
    {
        public MadeBetsDesign()
        {
            var items = Enumerable.Range(1, 60).Select(i => new Models.BetShortInfoModel
            {
                BetID = i * 1000 + i * 6548 - i * 745,
                BetDate = DateTime.Now,
                BetState = null,
                TotalBetAmount = i * 124,
                Content = new System.Collections.ObjectModel.ObservableCollection<Models.EventBetShortInfoModel>()
            })
            .ToArray();

            Populate(items);
        }
    }
}
