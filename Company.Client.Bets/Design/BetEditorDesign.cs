using System.Collections.ObjectModel;

namespace Company.Client.Bets.Design
{
    class BetEditorDesign
    {
        public dynamic Events { get; set; } = new EventsDesign();
        public dynamic Items { get; set; } 
        public dynamic SelectedItem { get; set; } 

        public BetEditorDesign()
        {
            Items = new ObservableCollection<BetDesign>()
            {
                new BetDesign(),
                new BetDesign(),
                new BetDesign()
            };

            SelectedItem = Items[1];
        }
    }
}
