using Company.Client.Common.Extensions;
using Company.Client.Common.Interfaces;
using Company.Wpf.Core.Utils;
using External.Types;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Company.Client.Bets.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class AccountViewModel : IDisposable
    {
        public event Action<AccountInfo> AccountChanged;

        public string Series { get; set; }
        public string Number { get; set; }
        public PersonalDocumentType SelectedDocumentType { get; set; }
        public DelegateCommand FindCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand RegistrationCommand { get; }
        public ObservableCollection<PersonalDocumentDescription> DocumentTypes { get; }
        [PropertyChanged.AlsoNotifyFor(nameof(AccountName))]
        public AccountInfo Account { get; private set; }
        public string AccountName => Account == null ? string.Empty : new FamilyName(Account.Surname, Account.Name, Account.Midname).ShortName;
        public BusyScope BusyScope { get; }


        private readonly IAccountIdentService _accountIdentService;
        private readonly IUserMessageService _userMessageService;
        private readonly ICardReader cardReader;

        public AccountViewModel(IAccountIdentService accountIdentService, IUserMessageService userMessageService, ICardReader cardReader)
        {
            _accountIdentService = accountIdentService;
            _userMessageService = userMessageService;

            if (cardReader != null)
            {
                this.cardReader = cardReader;
                this.cardReader.CardInserted += CardReaderCardInserted;
                this.cardReader.CardRemoved += CardReaderCardRemoved;
            }

            BusyScope = BusyScope.Default;
            DocumentTypes = new ObservableCollection<PersonalDocumentDescription>();
            CancelCommand = new DelegateCommand(Cancel, () => Account != null).ObservesProperty(() => Account);
            RegistrationCommand = new DelegateCommand(Registration);
            FindCommand = new DelegateCommand(TryFindByDocument);

            LoadData();
        }

        private void OnAccountChanged()
        {
            AccountChanged?.Invoke(Account);
        }
        private async void TryFindByDocument()
        {
            using (BusyScope.CreateWork())
            {
                try
                {
                    var result = await Task.Run(() => _accountIdentService.GetAccountByDocument(EPlayerType.Krm, SelectedDocumentType, Series, Number));
                    if (result == null)
                        _userMessageService?.ShowError("Акаунт не найден");
                    else
                        Account = result;
                }
                catch (Exception ex)
                {
                    _userMessageService?.ShowError(ex.GetAllErrors());
                }
            }
        }
        private async void TryFindAccountByCard(string cardNumber)
        {
            using (BusyScope.CreateWork())
            {
                try
                {
                    var account = await Task.Run(() => _accountIdentService.GetAccountByRFIDCard(cardNumber));
                    if (account == null)
                        Account = account;
                    else
                        _userMessageService?.ShowError("Аккаунт не найден");
                }
                catch (Exception ex)
                {
                    var message = ex.GetLastError();
                    _userMessageService?.ShowError(message);
                }
            }
        }
        private void Registration()
        {
            _userMessageService?.ShowWarring("Не доступно");
        }
        private void Cancel()
        {
            Account = null;
        }
        private async void LoadData()
        {
            using (BusyScope.CreateWork())
            {
                try
                {
                    var types = await Task.Run(() => _accountIdentService.GetPrimaryPersonalDocuments());
                    foreach (var type in types)
                        DocumentTypes.Add(type);
                }
                catch (Exception ex)
                {
                    var message = ex.GetLastError();
                    _userMessageService?.ShowError(message);
                }
            }

            if (cardReader != null)
            {
                var curentCard = cardReader.GetCardOnScanner();
                if (curentCard != null)
                    TryFindAccountByCard(curentCard.CardNumber);
            }
        }
        private void CardReaderCardRemoved(object sender, CardEventArgs args)
        {
            Account = null;
        }
        private void CardReaderCardInserted(object sender, CardEventArgs args)
        {
            TryFindAccountByCard(args.CardNumber);
        }
        public void Dispose()
        {
            if (cardReader != null)
            {
                cardReader.CardInserted -= CardReaderCardInserted;
                cardReader.CardRemoved -= CardReaderCardRemoved; 
            }
        }
    }
}
