using System;
using System.Linq;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Prism.Commands;
using Company.Client.Bets.Interfaces;
using Company.Client.Bets.Interfaces.Observers;
using Company.Client.Bets.Models;
using Company.Client.Common.Support;
using System.Collections;

namespace Company.Client.Bets.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class EventsViewModel: IEventInfoObserver, IDisposable
    {
        private readonly ObservableCollection<EventInfo> _eventsSource;
        private readonly DomainKindModel _zeroDomainKind;
        private readonly SearchWrapper _searchWrapper;
        private readonly IDisposable _subscription;

        private DomainKind? enabledDomainKind;

        public IList SearchParts => _searchWrapper.SearchParts;
        public string SearchText { get => _searchWrapper.SearchText; set => _searchWrapper.SearchText = value; }
        public ListCollectionView Events { get; }
        public ObservableCollection<DomainKindModel> DomainKinds { get; set; }
        public DelegateCommand<DomainKindModel> ApplyDomainKindCommand { get; set; }
        public DelegateCommand<EventInfo> SelectEventCommand { get; set; }

        public DelegateCommand ShowLiveEventsCommand { get; set; }
        public DelegateCommand ShowLineEventsCommand { get; set; }

        public bool ShowLiveEvents { get; set; }
        public bool ShowLineEvents { get; set; }

        public EventsViewModel(IDataService dataService, Action<EventInfo> selectedEventAction)
        {
            _eventsSource = new ObservableCollection<EventInfo>();
            _zeroDomainKind = new DomainKindModel { DomainKind = null, IsSelected = true };
            _searchWrapper = new SearchWrapper(() => Events.Refresh());

            Events = new ListCollectionView(_eventsSource);
            Events.Filter = EventFilter;
            DomainKinds = new ObservableCollection<DomainKindModel>() { _zeroDomainKind };
            ApplyDomainKindCommand = new DelegateCommand<DomainKindModel>(ApplyDomainKind);
            SelectEventCommand = new DelegateCommand<EventInfo>(selectedEventAction);
            ShowLiveEventsCommand = new DelegateCommand(() => { ShowLiveEvents = !ShowLiveEvents; Events.Refresh(); });
            ShowLineEventsCommand = new DelegateCommand(() => { ShowLineEvents = !ShowLineEvents; Events.Refresh(); });

            ShowLiveEvents = true;
            ShowLineEvents = true;

            _subscription = dataService.SubscribeOnEventsInfo(this);
        }

        internal void UpdateState(int[] eventsId)
        {
            foreach (var @event in _eventsSource)
                @event.IsIncludedInBet = eventsId.Contains(@event.ID);
        }

        private bool EventFilter(object eventInfo)
        {
            var sender = (EventInfo)eventInfo;
            var text = $"{sender.Number} {sender.Name}".ToLower();
            var condition1 = _searchWrapper.SearchParts.All(x => text.Contains(x));
            var condition2 = sender.IsLiveEvent ? ShowLiveEvents : ShowLineEvents;
            return (!enabledDomainKind.HasValue || sender.DomainKind == enabledDomainKind) && condition1 && condition2;
        }

        void IEventInfoObserver.PopulateEventInfo(EventInfo[] items)
        {
            foreach (var item in items.GroupBy(x => x.DomainKind).Select(x => x.Key))
            {
                DomainKinds.Add(new DomainKindModel { DomainKind = item, IsSelected = false });
            }

            foreach (var item in items)
            {
                _eventsSource.Add(item);
            }
        }

        private void ApplyDomainKind(DomainKindModel kind)
        {
            if (kind.IsSelected)
                return;

            enabledDomainKind = kind.DomainKind;
            kind.IsSelected = true;
            foreach (var domainkind in DomainKinds.Where(x => x != kind))
                domainkind.IsSelected = false;

            Events.Refresh();
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
