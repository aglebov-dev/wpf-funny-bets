using Company.Client.Bets.Models;
using Company.Client.Common.Interfaces;

namespace Company.Client.Bets.ViewModels
{
    class MadeBetViewModel
    {
        public string Name { get; }
        public BetInfoModel BetInfo { get; }

        private readonly IProcessingService ProcessingService;


        public MadeBetViewModel(IProcessingService processingService, BetInfoModel betinfo)
        {
            ProcessingService = processingService;
            BetInfo = betinfo;
            Name = $"№{betinfo.BetID}";
        }
    }
}
