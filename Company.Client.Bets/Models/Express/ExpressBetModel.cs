using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Company.Client.Bets.Interfaces.Observers;
using Company.Client.Common.Interfaces;
using FluentValidation;

namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    sealed class ExpressBetModel : BaseBetModel, IEventObserver
    {
        private IValidator ExpressBetValidator;
        private ObservableCollection<BetEventModel> EventsSource;

        public ReadOnlyObservableCollection<BetEventModel> Events { get; }

        public ExpressBetModel()
        {
            ExpressBetValidator = new ExpressBetEventValidator();
            EventsSource = new ObservableCollection<BetEventModel>();
            Events = new ReadOnlyObservableCollection<BetEventModel>(EventsSource);
        }

        public void PopulateEvent(BetEventModel eventModel)
        {
            EventsSource.Add(eventModel);
            UpdateBetEventValidator(eventModel, ExpressBetValidator);
        }
        public void RemoveEvent(BetEventModel eventModel)
        {
            EventsSource.Remove(eventModel);
            UpdateBetEventValidator(eventModel, null);
        }
        private void UpdateBetEventValidator(BetEventModel eventModel, IValidator validator = null)
        {
            eventModel.SetValidator(validator);
            InvalidateErrors();

            if (validator != null)
            {
                eventModel.ErrorsChanged += EventModelErrorsChanged;
            }
            else
            {
                eventModel.ErrorsChanged -= EventModelErrorsChanged;
            }
        }

        private void EventModelErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            InvalidateErrors();
        }

        public ExpressBet CreateExpressBet(int accountID, bool isCashPayment)
        {
            return new ExpressBet
            {
                AccountID = accountID,
                IsCashPayment = isCashPayment,
                Events = Events.Select(betEvent => new ExpressBetEvent {
                    EventID = betEvent.ID,
                    CoefficientID = betEvent.SelectedCoefficient.Id
                })
                .ToArray()
            };
        }
    }
}
