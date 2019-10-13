namespace Company.Client.Bets.Models
{
    class CoefficientModel
    {
        public long Id { get; set; }
        public float Value { get; set; }
        public OutcomeModel Outcome { get; set; }
    }
}
