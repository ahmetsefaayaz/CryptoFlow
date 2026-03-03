using CryptoFlow.Application.Dtos.DepositDtos;
using FluentValidation;

namespace CryptoFlow.Application.FluentValidation.DepositValidations;

public class DepositValidatior: AbstractValidator<DepositDto>
{
    public DepositValidatior()
    {
        RuleFor(d => d.Amount)
            .NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0).WithMessage("Amount must be greater than 0");
    }
}