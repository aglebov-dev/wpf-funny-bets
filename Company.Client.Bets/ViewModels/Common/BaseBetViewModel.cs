using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using Automatonymous;
using Prism.Commands;
using Company.Client.Bets.Models;
using Company.Client.Bets.Sagas;
using Company.Client.Common.Interfaces;
using External.Types;
using Company.Wpf.Core.Utils;

namespace Company.Client.Bets.ViewModels
{
    abstract class BaseBetViewModel
    {
        public string Name { get; }
        public BetState State { get; }
        public AccountViewModel Account { get; private set; }

        public BaseBetViewModel(IAccountIdentService accountIdentService)
        {
            Name = "НОВАЯ СТАВКА";
            State = new BetState();
            Account = new AccountViewModel(accountIdentService, null, null);
        }

        public void UpdateAccount(AccountViewModel account) => Account = account;
    }

    [PropertyChanged.AddINotifyPropertyChangedInterface]
    abstract class BaseBetViewModel<Tbet>: BaseBetViewModel, StateObserver<SaleBetSaga>, IDisposable where Tbet: BaseBetModel
    {
        private SaleBetStateMachine saleMachine;
        private InstanceLift<SaleBetStateMachine> saleMachineLift;
        protected BusyScope busyScope;

        public object Popup { get; private set; }
        public Tbet Bet { get; private set; }
        public DelegateCommand<Tbet> PrintCommand { get; }
        public DelegateCommand<Tbet> DetailsPrintCommand { get; }
        public DelegateCommand<Tbet> CancelBetCommand { get; }
        public DelegateCommand<Tbet> SaleBetCommand { get; }

        private readonly IBetReceiptProvider ReceiptService;
        private readonly IProcessingService BetProcessingService;
        private readonly IUserMessageService UserMessageService;

        public BaseBetViewModel(IAccountIdentService accountIdentService, IProcessingService betProcessingService, IUserMessageService userMessageService) 
            : base(accountIdentService)
        {
            ReceiptService = betProcessingService?.BetReceiptProvider;
            BetProcessingService = betProcessingService;
            UserMessageService = userMessageService;
            PrintCommand = new DelegateCommand<Tbet>(PrintBetReceipt, CanExecuteCommand);
            DetailsPrintCommand = new DelegateCommand<Tbet>(DetailsPrint, CanExecuteCommand);
            CancelBetCommand = new DelegateCommand<Tbet>(CancelBet, CanExecuteCommand);
            SaleBetCommand = new DelegateCommand<Tbet>(SaleBet, CanExecuteCommand);
            busyScope = BusyScope.Default;

            busyScope.PropertyChanged += (sender, args) => State.IsBusy = busyScope.IsBusy;
            Account.AccountChanged += AccountChanged;

            saleMachine = new SaleBetStateMachine(BetProcessingService);
            saleMachineLift = saleMachine.CreateInstanceLift(new SaleBetSaga());
            saleMachine.ConnectStateObserver(this);
        }

        private bool CanExecuteCommand(Tbet bet) => Account.Account != null && (bet != null && !bet.HasErrors);
        protected void SetBet(Tbet bet)
        {
            if (Bet != null) Bet.ErrorsChanged -= BetErrorsChanged;
            if (bet != null) bet.ErrorsChanged += BetErrorsChanged;
            Bet = bet;
        }
        protected void SetPopup(object viewModel)
        {
            Popup = viewModel;
            State.IsLock = Popup != null;
        }
        protected void ReleasePopup() => SetPopup(null);
        private void BetErrorsChanged(object sender, DataErrorsChangedEventArgs e) => RaiseCanExecuteCommands();
        private void AccountChanged(AccountInfo account) => RaiseCanExecuteCommands();
        private void RaiseCanExecuteCommands()
        {
            PrintCommand.RaiseCanExecuteChanged();
            DetailsPrintCommand.RaiseCanExecuteChanged();
            CancelBetCommand.RaiseCanExecuteChanged();
            SaleBetCommand.RaiseCanExecuteChanged();
        }
        private void PrintBetReceipt(Tbet data) => ReceiptService.PrintBetReceipt(data);
        private void DetailsPrint(Tbet data) => ReceiptService.PrintBetDetails(data);
        private void CancelBet(Tbet data) => BetProcessingService.CancelBet(data.BetID.Value);

        private void SaleBet(Tbet data)
        {
            if (data is ExpressBetModel)
            {
                var bet = (data as ExpressBetModel).CreateExpressBet(Account.Account.AccountId, true);
                var busy = busyScope.CreateWork();
                SaleAsync(bet).ContinueWith(task => { /* do something */ busy.Dispose(); });
            }
        }

        private async Task SaleAsync(BaseBet bet) 
            => await saleMachineLift.Raise(x => x.Begin, bet);

        Task StateObserver<SaleBetSaga>.StateChanged(InstanceContext<SaleBetSaga> context, State currentState, State previousState)
        {
            var states = new[] {
                saleMachine.FailureRemoteProcess,
                saleMachine.FailurePrintCashReceipt,
                saleMachine.FailureConfirmBet,
                saleMachine.FailurePrintReceipt
            };

            if (states.Contains(currentState))
            {
                var askViewModel = new AskViewModel("Ошибка", $"{context.Instance.FailureMessage}\nПовторить?", Continue);
                SetPopup(askViewModel);
            }
            else if (currentState == saleMachine.FailureBet)
            {
                //State.IsFailure = true;
                UserMessageService.ShowWarring($"{context.Instance.FailureMessage}");
            }
            return Task.CompletedTask;
        }

        private void Continue(bool continueUnswer)
        {
            ReleasePopup();
            var busy = busyScope.CreateWork();
            ContinueAsync(continueUnswer).ContinueWith(task => { /* do something */ busy.Dispose(); });
        }

        private async Task ContinueAsync(bool unswer)
            => await (unswer ? saleMachineLift.Raise(machine => machine.Continue) : saleMachineLift.Raise(machine => machine.RollBack));

        public void Dispose()
        {
            saleMachineLift.Raise(machine => machine.RollBack);
            OnDispose();
        }
        public virtual void OnDispose() { }
    }
}
