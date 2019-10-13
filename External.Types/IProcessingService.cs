namespace Company.Client.Common.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Обработка ставок
    /// </summary>
    public interface IProcessingService
    {
        /// <summary>
        /// Обработать ставку обычная/экспресс
        /// </summary>
        Task<int> SaleExpressBet(ExpressBet betData);
        /// <summary>
        /// Завершение процесса приема ставки
        /// </summary>
        Task FinalyzeBet(ApplyBetData data);
        /// <summary>
        /// Обработать ставку суперэкспресс
        /// </summary>
        Task<int> SaleSuperexpressBet(SuperexpressBet betData);
        /// <summary>
        /// Отменить ставку (аннулирование ставки в случае сбоя)
        /// </summary>
        Task CancelBet(int betID);
        /// <summary>
        /// Аннулировать ставку
        /// </summary>
        Task InvalidateBet(int betID);

        IPersistenceProvider PersistenceProvider { get; }
        IBetReceiptProvider BetReceiptProvider { get; }
    }
    /// <summary>
    /// Данные электронного чека
    /// </summary>
    public struct EctronicCashReceiptData
    {
        public bool IsEnabled { get; }
        public string Data { get; }
        public EctronicCashReceiptData(string emailOrPhone)
        {
            IsEnabled = true;
            Data = emailOrPhone;
        }
    }
    /// <summary>
    /// Ставка
    /// </summary>
    public class BaseBet
    {
        /// <summary>
        /// Ставка
        /// </summary>
        public int? BetID { get; set; }
        /// <summary>
        /// Аккаунт
        /// </summary>
        public int AccountID { get; set; }
        /// <summary>
        /// Оплата наличными
        /// </summary>
        public bool IsCashPayment { get; set; }
        /// <summary>
        /// Сумма ставки
        /// </summary>
        public decimal BetAmount { get; set; }
        /// <summary>
        /// Серийный номер ККМ
        /// </summary>
        public string KKMSerialNumber { get; set; }
        /// <summary>
        /// Данные для электронного чека
        /// </summary>
        public EctronicCashReceiptData EctronicCashReceiptData { get; set; }
    }
    /// <summary>
    /// Ставка суперэкспресс
    /// </summary>
    public class SuperexpressBet : BaseBet
    {
        /// <summary>
        /// Ставка
        /// </summary>
        public SuperexpressBetEvent[] Events { get; set; }
    }
    /// <summary>
    /// Ставка обычная/экспресс
    /// </summary>
    public class ExpressBet : BaseBet
    {
        /// <summary>
        /// Ставка
        /// </summary>
        public ExpressBetEvent[] Events { get; set; }
    }
    /// <summary>
    /// Событие - коэффициент
    /// </summary>
    public class ExpressBetEvent
    {
        public int EventID { get; set; }
        public long CoefficientID { get; set; }
    }
    /// <summary>
    /// Событие - коэффициент
    /// </summary>
    public class SuperexpressBetEvent
    {
        public int EventID { get; set; }
        public long[] CoefficientsID { get; set; }
    }
    public class ApplyBetData
    {
        public int BetID { get; set; }
        public string KKMSerialNumber { get; set; }
    }
}
