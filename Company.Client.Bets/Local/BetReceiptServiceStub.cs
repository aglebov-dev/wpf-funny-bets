using Company.Client.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace Company.Client.Bets.Local
{
    class BetReceiptServiceStub : IBetReceiptProvider
    {
        public string GetKKMSerialNumber() => "777-KKM-888";

        public Task PrintBetDetails<TData>(TData data)
        {
            throw new NotImplementedException();
        }

        public Task PrintBetReceipt<TData>(TData data)
        {
            return Task.CompletedTask;
        }

        public async Task PrintKKM(BaseBet data)
        {
            var betID = data.BetID;
            var betAmount = data.BetAmount;
            var isCash = data.IsCashPayment;
            var emailOrPhone = data.EctronicCashReceiptData.IsEnabled ? data.EctronicCashReceiptData.Data : null;

            await Task.Delay(1000);
        }
    }
}
