using System;
using System.Linq;
using System.Collections.Generic;
using Prism.Commands;
using Company.Client.Bets.Interfaces;
using Company.Client.Bets.Interfaces.Observers;
using Company.Client.Bets.Models;
using Company.Client.Common.Interfaces;

namespace Company.Client.Bets.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class BetViewModel : BaseBetViewModel<ExpressBetModel>, IEventObserver
    {
        public event Action<BetViewModel, BetEventModel[]> EventCollectionChanged;
        public DelegateCommand<BetEventModel> RemoveEventCommand { get; }

        private readonly IDataService DataService;
        private readonly Dictionary<int, IDisposable> Subscriptions;

        public BetViewModel(IDataService dataService, IAccountIdentService accountIdentService, IProcessingService betProcessingService, IUserMessageService userMessageService)
            : base(accountIdentService, betProcessingService, userMessageService)
        {
            DataService = dataService;
            Subscriptions = new Dictionary<int, IDisposable>();

            SetBet(new ExpressBetModel());
            Bet.SetValidator(new ExpressBetValidator());
            RemoveEventCommand = new DelegateCommand<BetEventModel>(RemoveEvent);
        }

        internal void HandleEventInfo(EventInfo eventInfo)
        {
            if (!Subscriptions.ContainsKey(eventInfo.ID))
                Subscriptions.Add(eventInfo.ID, DataService.SubscribeOnEvent(eventInfo.ID, this));
        }

        void IEventObserver.PopulateEvent(BetEventModel eventModel)
        {
            if (!State.IsBusy && !State.IsFailure && Popup == null)
            {
                Bet.PopulateEvent(eventModel);
                EventCollectionChanged?.Invoke(this, Bet.Events.ToArray());
            }
        }

        private void RemoveEvent(BetEventModel eventModel)
        {
            var key = eventModel.ID;
            if (Subscriptions.ContainsKey(key))
            {
                Subscriptions[key].Dispose();
                Subscriptions.Remove(key);
            }

            Bet.RemoveEvent(eventModel);
            EventCollectionChanged?.Invoke(this, Bet.Events.ToArray());
        }
        public override void OnDispose()
        {
            base.OnDispose();
            EventCollectionChanged = null;
        }
    }
}
