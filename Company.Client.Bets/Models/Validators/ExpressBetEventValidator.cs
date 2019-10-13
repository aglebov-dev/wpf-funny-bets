using FluentValidation;

namespace Company.Client.Bets.Models
{
    class ExpressBetEventValidator : AbstractValidator<BetEventModel>
    {
        public ExpressBetEventValidator()
        {
            RuleFor(x => x.SelectedCoefficient)
                .NotNull()
                .WithMessage("В событие не выбранн исход.");
        }
    }
}
