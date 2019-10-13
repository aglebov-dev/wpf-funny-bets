using System;
using System.Threading.Tasks;
using Company.Client.Bets.Services;
using Company.Client.Common.Interfaces;

namespace Company.Client.Bets.Local
{
    class BetProcessingServiceStub : IProcessingService
    {
        public IPersistenceProvider PersistenceProvider { get; } = new PersistenceProvider();
        public IBetReceiptProvider BetReceiptProvider { get; } = new BetReceiptServiceStub();


        public BetProcessingServiceStub()
        {
        }


        public Task CancelBet(int betID)
        {
            return Task.CompletedTask;
        }

        public Task InvalidateBet(int betID)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaleExpressBet(ExpressBet betData)
        {
            await Task.Delay(1000);
            return 777;
        }

        public Task<int> SaleSuperexpressBet(SuperexpressBet betData)
        {
            throw new NotImplementedException();
        }

        public Task FinalyzeBet(ApplyBetData data)
        {
            throw new NotImplementedException();
        }
    }

}
