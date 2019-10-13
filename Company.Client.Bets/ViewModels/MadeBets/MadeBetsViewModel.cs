using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Company.Client.Bets.Models;
using Company.Client.Bets.Interfaces;
using Company.Client.Bets.Interfaces.Observers;

namespace Company.Client.Bets.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class MadeBetsViewModel: IMadeBetsObserver
    {
        public string SearchText { get; set; } = "Hello";
        public ObservableCollection<BetShortInfoModel> Bets { get; set; }
        public DelegateCommand<BetShortInfoModel> SelectedMadeBetCommand { get; }

        internal MadeBetsViewModel()
        {
            Bets = new ObservableCollection<BetShortInfoModel>();
        }
        public MadeBetsViewModel(IDataService dataService, Action<BetShortInfoModel> selectedMadeBet) : this()
        {
            dataService.SubscribeOnMadeBets(this);
            SelectedMadeBetCommand = new DelegateCommand<BetShortInfoModel>(selectedMadeBet);
        }

        public void Populate(BetShortInfoModel[] items)
        {
            Bets.Clear();
            foreach (var item in items)
            {
                Bets.Add(item);
            }
        }
    }
}
