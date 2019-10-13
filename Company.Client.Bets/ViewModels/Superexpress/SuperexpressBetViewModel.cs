using Company.Client.Bets.Interfaces;
using Company.Client.Bets.Interfaces.Observers;
using Company.Client.Bets.Models;
using Company.Client.Common.Interfaces;
using Company.Client.Common.Support;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Company.Client.Bets.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class EventViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public ObservableCollection<CoefficientViewModel> Coefficients { get; }

        public EventViewModel()
        {
            Coefficients = new ObservableCollection<CoefficientViewModel>();
        }
    }

    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class CoefficientViewModel
    {
        public event Action<CoefficientViewModel> SelectedChanged;

        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }


        void OnIsSelectedChanged()
            => SelectedChanged?.Invoke(this);
    }

    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class SuperexpressBetViewModel : BaseBetViewModel<SuperexpressBetModel>, ISuperexpressDrawObserver
    {
        public event Action<SuperexpressBetViewModel> DrawChanged;

        private SuperexpressDrawInfoModel _info;

        /// <summary>
        /// Событие
        /// </summary>
        public event Action<CoefficientViewModel> CoefficientSelected;

        /// <summary>
        /// Тираж
        /// </summary>
        public int DrawId => _info?.Id ?? 0;

        /// <summary>
        /// Ошибки
        /// </summary>
        public bool HasErrors => !int.TryParse(BetAmount, out var betamount) || betamount < MinBetAmount || VariantsAmount <= 0;

        /// <summary>
        /// Кол-во вариантов
        /// </summary>
        [PropertyChanged.AlsoNotifyFor(nameof(MinBetAmount), nameof(HasErrors), nameof(MinBetToolTip))]
        public int VariantsAmount { get; private set; }

        /// <summary>
        /// Сумма ставки
        /// </summary>
        [PropertyChanged.AlsoNotifyFor(nameof(HasErrors))]
        public string BetAmount { get; set; }
        /// <summary>
        /// Сумма ставки
        /// </summary>
        public decimal BetAmountOriginal => int.TryParse(BetAmount, out var betamount) ? betamount : 0;


        /// <summary>
        /// Базовая стоимость
        /// </summary>
        public decimal BaseAmount => _info?.BaseAmount ?? 0;

        /// <summary>
        /// Минимальная сумма ставки
        /// </summary>
        public decimal MinBetAmount => _info != null ? Math.Max(_info.MinBetAmount, _info.BaseAmount * VariantsAmount) : 0;

        /// <summary>
        /// Подсказка к полю минимальной ставки
        /// </summary>
        public string MinBetToolTip => $"минимальная ставка"; //  =  {VariantsAmount} X {_info.MinBetAmount:C0}

        /// <summary>
        /// Оплата бонусными балами
        /// </summary>
        public bool IsBonusPay { get; set; }

        /// <summary>
        /// События
        /// </summary>
        public ObservableCollection<EventViewModel> Events { get; }

        /// <summary>
        /// Динамические заголовки
        /// </summary>
        public ObservableCollection<string> Headers { get; }

        public DelegateCommand<CoefficientViewModel> SelectOutcomeCommand { get; }
        public DelegateCommand RandomGameCommand { get; }

        private readonly IDataService _dataService;
        private IDisposable _subscription;

        public SuperexpressBetViewModel(IDataService dataService, IAccountIdentService accountIdentService, IProcessingService betProcessingService, IUserMessageService userMessageService)
            : base(accountIdentService, betProcessingService, userMessageService)
        {
            _dataService = dataService;

            Headers = new ObservableCollection<string>();
            Events = new ObservableCollection<EventViewModel>();

            RandomGameCommand = new DelegateCommand(RandomGame);
            SelectOutcomeCommand = new DelegateCommand<CoefficientViewModel>(SelectOutcome);
        }

        private void SelectOutcome(CoefficientViewModel coefficient)
        {
            if (coefficient != null)
                coefficient.IsSelected = true;
        }

        internal void HandleDrawInfo(SuperexpressDrawInfoModel drawInfo)
        {
            if (_info?.Id == drawInfo.Id)
                return;

            if (_subscription != null)
                _subscription.Dispose();

            //create work

            _info = drawInfo;
            _subscription = _dataService.SubscribeOnDraw(drawInfo.Id, this);
        }
        void ISuperexpressDrawObserver.PopulateDraw(SuperexpressDrawModel model)
        {
            //dispose work
            Fill(model, _info);
        }

        private void RandomGame()
        {
            var count = Events.FirstOrDefault()?.Coefficients.Count;
            if (count.HasValue)
            {
                foreach (var coefficient in Events.SelectMany(x => x.Coefficients))
                    coefficient.IsSelected = false;

                var indexes = RandomGenerator.GenerateInteger(0, count.Value, Events.Count);
                for (int i = 0; i < indexes.Length; i++)
                {
                    var choose = indexes[i];
                    Events[i].Coefficients[choose].IsSelected = true;
                }
            }
        }

        private void Fill(SuperexpressDrawModel draw, SuperexpressDrawInfoModel drawInfo)
        {
            Headers.Clear();
            Events.Clear();

            var outcomesIds = draw.Events
                  .SelectMany(x => x.Coefficients)
                  .Select(x => x.Outcome)
                  .Select(x => x.Id)
                  .Distinct();

            foreach (var outcome in outcomesIds)
            {
                var outcomeName = draw.Events.SelectMany(x => x.Coefficients).FirstOrDefault(x => x.Outcome.Id == outcome).Outcome.Name;
                Headers.Add(outcomeName);
            }


            foreach (var item in draw.Events)
            {
                var ev = new EventViewModel
                {
                    Id = item.ID,
                    Name = item.Name,
                    Path = item.BranchPathName
                };

                foreach (var outcome in outcomesIds)
                {
                    var coefficient = item.Coefficients
                        .Where(x => x.Outcome.Id == outcome)
                        .Select(x => new CoefficientViewModel
                        {
                            Id = x.Id,
                            Name = x.Outcome.Name,
                            IsSelected = false
                        })
                        .FirstOrDefault();

                    if (coefficient != null)
                        coefficient.SelectedChanged += CoefficientSelectedChanged;

                    ev.Coefficients.Add(coefficient);
                }

                Events.Add(ev);
            }

            CalculateVariantsAmount();

            DrawChanged?.Invoke(this);
        }

        private void CoefficientSelectedChanged(CoefficientViewModel obj)
        {
            CalculateVariantsAmount();
            CoefficientSelected?.Invoke(obj);
        }
        void CalculateVariantsAmount()
        {
            VariantsAmount = Events.Select(x => x.Coefficients.Count(j => j?.IsSelected ?? false)).Aggregate(1, (s, x) => s *= x);
        }
    }
}
