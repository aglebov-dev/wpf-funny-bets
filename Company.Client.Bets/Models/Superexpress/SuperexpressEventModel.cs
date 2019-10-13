namespace Company.Client.Bets.Models
{
    /// <summary>
    /// Событие суперэкспресса
    /// </summary>
    class SuperexpressEventModel : EventBaseModel
    {
        public string BranchPathName { get; set; }
        public CoefficientModel[] Coefficients { get; set; }
    }
}
