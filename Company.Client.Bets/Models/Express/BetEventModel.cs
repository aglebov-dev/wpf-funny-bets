using System.Collections.ObjectModel;

namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class BetEventModel: EventBaseModel
    {
        public decimal MaxAmount { get; set; }
        public ObservableCollection<TimeIntervalModel> TimeIntervals { get; set; }
        public ObservableCollection<CoefficientModel> Coefficients { get; set; }
        public TimeIntervalModel SelectedTimeInterval { get; set; }
        public CoefficientModel SelectedCoefficient { get; set; }
    }
}
