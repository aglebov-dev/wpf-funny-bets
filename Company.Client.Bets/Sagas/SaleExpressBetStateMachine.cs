using System;
using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Binders;
using Company.Client.Bets.Services;
using Company.Client.Common.Interfaces;
using External.Types;

namespace Company.Client.Bets.Sagas
{
    class SaleBetStateMachine : AutomatonymousStateMachine<SaleBetSaga>
    {
        public State ReadyForSale { get; set; }
        public State PrintCashReceipt { get; set; }
        public State PrintReceipt { get; set; }
        public State RemoteProcess { get; set; }
        public State ConfirmBet { get; set; }
        public State FailureRemoteProcess { get; set; }
        public State FailurePrintReceipt { get; set; }
        public State FailurePrintCashReceipt { get; set; }
        public State FailureConfirmBet { get; set; }
        public State FailureBet { get; set; }
        public Event<BaseBet> Begin { get; set; }
        public Event Continue { get; set; }
        public Event RollBack { get; set; }
        public Event<string> Restore { get; set; }

        private const string TOPIC = "sale-bet-state-machine";
        private readonly IProcessingService ProcessingService;
        private readonly IBetReceiptProvider ReceiptProvider;
        private readonly JsonStateSerializer<SaleBetStateMachine, SaleBetSaga> JsonStateSerializer;

        public SaleBetStateMachine(IProcessingService processingService)
        {
            ProcessingService = processingService;
            ReceiptProvider = processingService?.BetReceiptProvider;
            JsonStateSerializer = new JsonStateSerializer<SaleBetStateMachine, SaleBetSaga>(this);
            
            BuildStateMachine();
        }

        private void BuildStateMachine()
        {
            During(Initial, When(Begin)
                .Then(InitialSetup)
                .TransitionTo(RemoteProcess));

            During(Initial, When(Restore).Then(RestoreInternal));

            WhenEnter(RemoteProcess, binder => binder
                .ThenAsync(RemoteProcessingHandler)
                .Catch<BetException>(BetExceptionHandler)
                .Catch<Exception>(ex => DefaultExceptionHandler(ex, FailureRemoteProcess))
                .If(context => context.Instance.OrderWasHandled, binderIf => binderIf.TransitionTo(PrintCashReceipt)));

            WhenEnter(PrintCashReceipt, binder => binder
                .ThenAsync(KKMHandler)
                .Catch<Exception>(ex => DefaultExceptionHandler(ex, FailurePrintCashReceipt))
                .If(context => context.Instance.CashReceiptPrinted, binderIf => binderIf.TransitionTo(PrintReceipt)));

            WhenEnter(PrintReceipt, binder => binder
                .ThenAsync(PrintingReceipt)
                .Catch<Exception>(ex => DefaultExceptionHandler(ex, FailurePrintReceipt))
                .If(context => context.Instance.ReceiptPrinted, binderIf => binderIf.TransitionTo(ConfirmBet)));

            WhenEnter(ConfirmBet, binder => binder
                .ThenAsync(ConfirmBetHandler)
                .Catch<Exception>(ex => DefaultExceptionHandler(ex, FailureConfirmBet))
                .If(context => context.Instance.OrderWasConfirmed, binderIf => binderIf.TransitionTo(Final)));

            WhenEnter(FailureBet, binder => binder.TransitionTo(Initial));

            //Continue
            During(FailureRemoteProcess, When(Continue).TransitionTo(RemoteProcess));
            During(FailurePrintCashReceipt, When(Continue).TransitionTo(PrintCashReceipt));
            During(FailurePrintReceipt, When(Continue).TransitionTo(PrintReceipt));
            During(FailureConfirmBet, When(Continue).TransitionTo(ConfirmBet));

            //RollBack
            During(FailureRemoteProcess, When(RollBack).TransitionTo(Initial));
            During(FailurePrintCashReceipt, When(RollBack).ThenAsync(RollBackThenOrderWasHandled).TransitionTo(Initial));
            During(FailurePrintReceipt, When(RollBack).ThenAsync(RollBackThenOrderWasHandled).TransitionTo(Initial));
            During(FailureConfirmBet, When(RollBack).ThenAsync(RollBackThenOrderWasHandled).TransitionTo(Initial));
        }
        private void InitialSetup(EventContext<SaleBetSaga, BaseBet> context)
        {
            context.Instance.Bet = context.Data;
        }
        private ExceptionActivityBinder<SaleBetSaga, Exception> DefaultExceptionHandler(ExceptionActivityBinder<SaleBetSaga, Exception> ex, State state)
        {
            return ex
                .Then(WriteException)
                .TransitionTo(state)
                .Then(async context => await WriteState(context));
        }
        private ExceptionActivityBinder<SaleBetSaga, BetException> BetExceptionHandler(ExceptionActivityBinder<SaleBetSaga, BetException> ex)
        {
            var value = ex.Then(WriteException)
                .TransitionTo(FailureBet)
                .Then(async context => await WriteState(context));

            return value;
        }
        private void WriteException(BehaviorExceptionContext<SaleBetSaga, Exception> context)
        {
            context.Instance.FailureMessage = context.Exception.Message;
            context.Instance.Exception = context.Exception;
        }
        private async Task RemoteProcessingHandler(BehaviorContext<SaleBetSaga> context)
        {
            context.Instance.Bet.KKMSerialNumber = ReceiptProvider.GetKKMSerialNumber();
            await WriteState(context);

            if (context.Instance.Bet is ExpressBet)
            {
                var betID = await ProcessingService.SaleExpressBet((ExpressBet)context.Instance.Bet);
                context.Instance.SetBetID(betID);
                await WriteState(context);
            }
            else
            {
                throw new Exception();
            }
        }
        private async Task KKMHandler(BehaviorContext<SaleBetSaga> context)
        {
            context.Instance.Bet.KKMSerialNumber = ReceiptProvider.GetKKMSerialNumber();
            await WriteState(context);
            await ReceiptProvider.PrintKKM(context.Instance.Bet);
            context.Instance.CashReceiptPrinted = true;
            await WriteState(context);
        }
        private async Task PrintingReceipt(BehaviorContext<SaleBetSaga> context)
        {
            await WriteState(context);
            await ReceiptProvider.PrintBetReceipt(context.Instance.Bet);
            context.Instance.ReceiptPrinted = true;
            await WriteState(context);
        }
        private async Task ConfirmBetHandler(BehaviorContext<SaleBetSaga> context)
        {
            await WriteState(context);
            var data = new ApplyBetData {
                BetID = context.Instance.Bet.BetID.Value,
                KKMSerialNumber = context.Instance.Bet.KKMSerialNumber
            };

            await ProcessingService.FinalyzeBet(data);
            context.Instance.OrderWasConfirmed = true;
            await RemoveState(context);
        }
        private async Task RollBackThenOrderWasHandled(BehaviorContext<SaleBetSaga> context)
        {
            await ProcessingService.CancelBet(context.Instance.Bet.BetID.Value);
            context.Instance.FailureMessage = $"Ставка {context.Instance.Bet.BetID:n0} отменена.";
        }
        private async Task WriteState(BehaviorContext<SaleBetSaga> context)
        {
            var json = JsonStateSerializer.Serialize(context.Instance);
            await ProcessingService.PersistenceProvider.Write(TOPIC, context.Instance.PersistenceID, json);
        }
        private async Task RemoveState(BehaviorContext<SaleBetSaga> context)
        {
            await ProcessingService.PersistenceProvider.Delete(TOPIC, context.Instance.PersistenceID);
        }

        private void RestoreInternal(BehaviorContext<SaleBetSaga, string> context)
        {
            var value = JsonStateSerializer.Deserialize<SaleBetSaga>(context.Data);
            context.Instance.Restore(value);

            this.InstanceState(x => value.CurentState);
        }
    }
}
