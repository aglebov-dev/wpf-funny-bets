using System;
using System.Linq;
using System.Collections.ObjectModel;
using Prism.Commands;
using Company.Client.Bets.Interfaces;
using Company.Client.Bets.Interfaces.Observers;
using Company.Client.Bets.Models;
using Company.Client.Common.Support;
using System.Collections;
using System.Windows.Data;

namespace Company.Client.Bets.ViewModels
{
    class SuperexpressDrawsViewModel : ISuperexpressDrawsInfoObserver, IDisposable
    {
        private readonly IDisposable DrawInfoSubscription;
        private readonly SearchWrapper SearchWrapper;
        private readonly ObservableCollection<SuperexpressDrawInfoModel> DrawsSource;

        public IList SearchParts => SearchWrapper.SearchParts;
        public string SearchText { get => SearchWrapper.SearchText; set => SearchWrapper.SearchText = value; }
        public ListCollectionView Draws { get; protected set; }
        public DelegateCommand<SuperexpressDrawInfoModel> SelectDrawCommand { get; set; }

        public SuperexpressDrawsViewModel(IDataService dataService, Action<SuperexpressDrawInfoModel> selectedDrawAction)
        {
            SearchWrapper = new SearchWrapper(() => Draws.Refresh());
            DrawsSource = new ObservableCollection<SuperexpressDrawInfoModel>();
            Draws = new ListCollectionView(DrawsSource);
            Draws.Filter = DrawsFilter;

            if (selectedDrawAction != null)
                SelectDrawCommand = new DelegateCommand<SuperexpressDrawInfoModel>(selectedDrawAction);

            if (dataService != null)
                DrawInfoSubscription = dataService.SubscribeOnDrawsInfo(this);

        }
        private bool DrawsFilter(object eventInfo)
        {
            var sender = (SuperexpressDrawInfoModel)eventInfo;
            var text = $"{sender.DrawCode} {sender.Name}".ToLower();
            var condition = SearchWrapper.SearchParts.All(x => text.Contains(x));

            return condition;
        }
        public void Invalidate(SuperexpressDrawInfoModel[] draws)
        {
            DrawsSource.Clear();
            foreach (var draw in draws)
                DrawsSource.Add(draw);
        }

        public void AppendDraw(SuperexpressDrawInfoModel draw)
        {
            RemoveDraw(draw.Id);
            DrawsSource.Add(draw);
        }

        public void UpdateDraw(SuperexpressDrawInfoModel draw)
        {
            //TODO
        }

        public void RemoveDraw(int drawID)
        {
            var existDraw = DrawsSource.FirstOrDefault(x => x.Id == drawID);
            if (existDraw != null)
                Draws.Remove(existDraw);
        }

        public void Dispose()
        {
            DrawInfoSubscription.Dispose();
        }

        internal void UpdateState(int? drawId)
        {
            foreach (var draw in DrawsSource)
                draw.IsSelected = draw.Id == drawId;
        }
    }
}
