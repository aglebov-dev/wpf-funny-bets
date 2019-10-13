namespace Company.Client.Bets.ViewModels
{
    class MainViewModel
    {
        public BetEditorViewModel BetEditor { get; set; }

        public MainViewModel()
        {
            BetEditor = new BetEditorViewModel(App.DataService, App.AccountIdentService, App.BetReceiptService, App.BetProcessingService, App.UserMassegeService);
        }
    }
}
