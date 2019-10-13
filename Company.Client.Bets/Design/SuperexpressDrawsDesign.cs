using System;
using System.Collections.ObjectModel;
using Company.Client.Bets.Models;
using Company.Client.Bets.ViewModels;

namespace Company.Client.Bets.Design
{
    class SuperexpressDrawsDesign : SuperexpressDrawsViewModel
    {
        public SuperexpressDrawsDesign()
            : base(null, null)
        {
            var items = new ObservableCollection<SuperexpressDrawInfoModel>();
            Draws = new System.Windows.Data.ListCollectionView(items);

            for (int i = 0; i < 12; i++)
            {
                var draw = new SuperexpressDrawInfoModel
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
                };

                items.Add(draw);
            }
        }
    }
}
