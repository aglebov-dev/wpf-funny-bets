using System.Windows;
using Company.Client.Bets.Interfaces;
using Company.Client.Bets.ViewModels;
using Company.Client.Bets.Desktop.Views;
using Company.Client.Common.Interfaces;

namespace Company.Client.Bets
{
    public partial class App : Application
    {
        internal static IAccountIdentService AccountIdentService { get; } = new Local.AccountServiceStub();
        internal static IDataService DataService { get; } = new Local.LocalDataService(); // new DataServiceStub();
        internal static IProcessingService BetProcessingService { get; } = new Local.BetProcessingServiceStub();
        internal static IBetReceiptProvider BetReceiptService { get; } = new Local.BetReceiptServiceStub();
        internal static IUserMessageService UserMassegeService { get; } = new Local.UserMassageServiceStub();

        protected override void OnStartup(StartupEventArgs e)
        {
            var view = new MainView
            {
                DataContext = new MainViewModel()
            }; 

            var window = new Window { Content = view };
            window.Show();
        }
    }
}
