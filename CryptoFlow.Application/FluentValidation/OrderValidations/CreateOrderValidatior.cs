using CryptoFlow.Application.Dtos.OrderDtos;
using FluentValidation;

namespace CryptoFlow.Application.FluentValidation.OrderValidations;

public class CreateOrderValidatior: AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidatior()
    {
        RuleFor(o => o.Quantity)
            .NotEmpty().WithMessage("Quantity is required")
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}