using System;
using System.Collections.ObjectModel;

namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class BetShortInfoModel: BaseBetModel
    {
        public string Number => $"{BetID:000000000}";
        public object BetState { get; set; } //принята,выиграла, проиграла, на подтверждении и т.д.
        public DateTime BetDate { get; set; }
        public ObservableCollection<EventBetShortInfoModel> Content { get; set; }
    }
}
