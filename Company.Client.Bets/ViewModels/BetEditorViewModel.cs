using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Company.Client.Bets.Models;
using Company.Client.Bets.Interfaces;
using System.Collections.Generic;
using Prism.Commands;
using System;
using Company.Client.Common.Interfaces;
using Company.Client.Common.Extensions;

namespace Company.Client.Bets.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class BetEditorViewModel
    {
        public EventsViewModel Events { get; set; }
        public SuperexpressDrawsViewModel Draws { get; set; }
        public MadeBetsViewModel MadeBets { get; set; }
        public bool IsShowMadeBets { get; set; }
        public object SelectedItem { get; set; }
        public ObservableCollection<object> Items { get; set; }       
        public DelegateCommand<object> RemoveBetCommand { get; }
        public DelegateCommand<BaseBetViewModel> SwitchToExpressCommand { get; }
        public DelegateCommand<BaseBetViewModel> SwitchToSystemCommand { get; }


        public DelegateCommand CreateExpressBetCommand { get; }
        public DelegateCommand CreateSuperexpressBetCommand { get; }

        private readonly IDataService DataService;
        private readonly IAccountIdentService AccountIdentService;
        private readonly IBetReceiptProvider ReceiptService;
        private readonly IProcessingService ProcessingService;
        private readonly IUserMessageService UserMessageService;

        public BetEditorViewModel(IDataService dataService, IAccountIdentService accountIdentService, IBetReceiptProvider receiptService, IProcessingService processingService, IUserMessageService userMessageService)
        {
            DataService = dataService;
            AccountIdentService = accountIdentService;
            ReceiptService = receiptService;
            ProcessingService = processingService;
            UserMessageService = userMessageService;

            Items = new ObservableCollection<object>();
            Events = new EventsViewModel(dataService, SelectedEventAction);
            Draws = new SuperexpressDrawsViewModel(dataService, SelectedDrawAction);
            MadeBets = new MadeBetsViewModel(DataService, SelectedMadeBet);

            RemoveBetCommand = new DelegateCommand<object>(RemoveBet);
            SwitchToExpressCommand = new DelegateCommand<BaseBetViewModel>(SwitchToExpress);
            SwitchToSystemCommand = new DelegateCommand<BaseBetViewModel>(SwitchToSystem, x => false);
            CreateExpressBetCommand = new DelegateCommand(CreateNewExpressBet);
            CreateSuperexpressBetCommand = new DelegateCommand(CreateNewSuperexpressBet);

            this.OfInpc(x => x.PropertyChanged += SelfPropertyChanged);
        }

        private void CreateNewExpressBet()
        {
            var express = CreateExpressBet();
            Items.Add(SelectedItem = express);
        }
        private void CreateNewSuperexpressBet()
        {
            var express = CreateSuperexpressBet();
            Items.Add(SelectedItem = express);
        }
        private void SwitchToExpress(BaseBetViewModel bet)
        {
            if (!(SelectedItem is BetViewModel))
            {
                var express = CreateExpressBet();
                express.UpdateAccount(bet.Account);

                var index = Items.IndexOf(bet);
                Items.Remove(bet);
                Items.Insert(index, express);
                SelectedItem = express;
            }
        }
        private void SwitchToSystem(BaseBetViewModel bet)
        {
            throw new NotImplementedException();
        }
        private BaseBetViewModel CreateExpressBet()
        {
            var bet = new BetViewModel(DataService, AccountIdentService, ProcessingService, UserMessageService);
            bet.EventCollectionChanged += BetEventCollectionChanged;
            return bet;
        }
        private BaseBetViewModel CreateSuperexpressBet()
        {
            var bet = new SuperexpressBetViewModel(DataService, AccountIdentService, ProcessingService, UserMessageService);
            bet.DrawChanged += BetDrawChanged;
            return bet;
        }
        private MadeBetViewModel CreateMadeBet(int betID)
        {
            var bet = DataService.GetMadeBetInfo(betID);
            var madeBet = new MadeBetViewModel(ProcessingService, bet);
            return madeBet;
        }

        private void RemoveBet(object bet)
        {
            if (SelectedItem == bet)
            {
                var index = Items.IndexOf(bet) - 1;
                SelectedItem = index >= 0 ? Items[index] : Items.FirstOrDefault();
            }

            (bet as IDisposable)?.Dispose();
            Items.Remove(bet);
        }
        private void SelfPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem))
            {
                UpdateEventsState(SelectedItem is BetViewModel ? (SelectedItem as BetViewModel).Bet.Events : Enumerable.Empty<BetEventModel>());
                UpdateDrawsState(SelectedItem is SuperexpressBetViewModel ? (SelectedItem as SuperexpressBetViewModel).DrawId : (int?)null);
            }
        }
        private void SelectedEventAction(EventInfo eventInfo)
        {
            if (!(SelectedItem is BetViewModel))
                CreateNewExpressBet();

            var curentBet = SelectedItem as BetViewModel;
            curentBet.HandleEventInfo(eventInfo);
        }
        private void SelectedDrawAction(SuperexpressDrawInfoModel drawInfo)
        {
            if (!(SelectedItem is SuperexpressBetViewModel))
                CreateNewSuperexpressBet();

            var curentBet = SelectedItem as SuperexpressBetViewModel;
            curentBet.HandleDrawInfo(drawInfo);
        }
        private void SelectedMadeBet(BetShortInfoModel madeBet)
        {
            if (!madeBet.BetID.HasValue)
                throw new ApplicationException("Что то пошло не так.");
           
            var madeBetViewModel = CreateMadeBet(madeBet.BetID.Value);
            Items.Add(SelectedItem = madeBetViewModel);
        }
        private void BetEventCollectionChanged(BetViewModel sender, IEnumerable<BetEventModel> events)
        {
            if (sender == SelectedItem)
                UpdateEventsState(events);
        }
        private void BetDrawChanged(SuperexpressBetViewModel sender)
        {
            if (sender == SelectedItem)
                UpdateDrawsState(sender.DrawId);
        }
        private void UpdateEventsState(IEnumerable<BetEventModel> events)
        {
            var eventsId = events.Select(x => x.ID).ToArray();
            Events.UpdateState(eventsId);
        }
        private void UpdateDrawsState(int? drawId)
        {
            Draws.UpdateState(drawId);
        }
    }
}
