using System;
using Automatonymous;
using Company.Client.Common.Interfaces;

namespace Company.Client.Bets.Sagas
{
    class SaleBetSaga
    {
        /// <summary> Состояние для стэйт-машины </summary>
        public State CurentState { get; set; }
        /// <summary> Распечатана квитанция </summary>
        public bool ReceiptPrinted { get; set; }
        /// <summary> Напечатан кассовый чек </summary>
        public bool CashReceiptPrinted { get; set; }
        /// <summary> Ставка обработана на сервере </summary>
        public bool OrderWasHandled { get; set; }
        /// <summary> Подтверждение клиентом, что все операции по ставке (ККМ, печать квитанции и т.п.) завершены </summary>
        public bool OrderWasConfirmed { get; set; }
        /// <summary> Ставка </summary>
        public BaseBet Bet { get; set; }
        /// <summary> Сообщение об ошибке </summary>
        public string FailureMessage { get; set; }
        /// <summary> Ошибка </summary>
        public Exception Exception { get; set; }
        /// <summary> Уникальный индентификатор записи </summary>
        public Guid PersistenceID { get; set; }

        public SaleBetSaga()
        {
            PersistenceID = Guid.NewGuid();
        }
        public void SetBetID(int betID)
        {
            OrderWasHandled = true;
            Bet.BetID = betID;
        }
        public void Restore(SaleBetSaga value)
        {
            CurentState        = value.CurentState;
            ReceiptPrinted     = value.ReceiptPrinted;
            CashReceiptPrinted = value.CashReceiptPrinted;
            OrderWasHandled    = value.OrderWasHandled;
            OrderWasConfirmed  = value.OrderWasConfirmed;
            Bet                = value.Bet;
            FailureMessage     = value.FailureMessage;
            Exception          = value.Exception;
            PersistenceID      = value.PersistenceID;
        }
    }
}
