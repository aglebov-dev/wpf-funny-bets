using FluentValidation;

namespace Company.Client.Bets.Models.Validators
{
    class SuperexpressBetValidator : AbstractValidator<SuperexpressBetModel>
    {
        public SuperexpressBetValidator()
        {
            RuleFor(x => x.TotalBetAmount)
                .GreaterThan(0)
                .WithMessage("Сумма ставки должна быть больше 0");
        }
    }
}
