namespace Company.Client.Common.Interfaces
{
    using System.Threading.Tasks;

    public interface IBetReceiptProvider
    {
        string GetKKMSerialNumber();

        Task PrintBetReceipt<TData>(TData data);
        Task PrintBetDetails<TData>(TData data);
        Task PrintKKM(BaseBet data);
    }
}
