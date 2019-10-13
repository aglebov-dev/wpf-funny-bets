using Company.Client.Common.Interfaces;
using System;
using System.Collections.ObjectModel;

namespace Company.Client.Bets.Local
{
    class UserMassageServiceStub : IUserMessageService
    {
        public ReadOnlyObservableCollection<UserMessage> Messages => throw new NotImplementedException();

        public void ShowConfirmation(string message)
        {
            System.Windows.MessageBox.Show(message, "CONFIRM", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public void ShowError(string message)
        {
            System.Windows.MessageBox.Show(message, "EXCEPTION", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }

        public void ShowInformation(string message)
        {
            System.Windows.MessageBox.Show(message, "INFO", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public void ShowWarring(string message)
        {
            System.Windows.MessageBox.Show(message, "ATTENTION", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
        }
    }
}
