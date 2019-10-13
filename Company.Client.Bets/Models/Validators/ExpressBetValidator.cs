using FluentValidation;

namespace Company.Client.Bets.Models
{
    class ExpressBetValidator: AbstractValidator<ExpressBetModel>
    {
        public ExpressBetValidator()
        {
            RuleFor(x => x.TotalBetAmount)
                .GreaterThan(0)
                .WithMessage("Сумма ставки должна быть больше 0");

            RuleForEach(x => x.Events)
                .Must(betEvent => !betEvent.HasErrors)
                .WithMessage("В одном или нескольких событиях не выбранны исход.");
        }
    }
}
