using CryptoFlow.Application.Dtos.AuthDtos;
using FluentValidation;

namespace CryptoFlow.Application.FluentValidation.AuthValidation;

public class LoginValidator: AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must have at least 6 characters")
            .MaximumLength(100).WithMessage("Password cant be over 100 characters")
            .Matches("[A-Z]").WithMessage("Password must have at 1 Uppercase")
            .Matches("[a-z]").WithMessage("Password must have at 1 Lowercase")
            .Matches("[0-9]").WithMessage("Password must have at 1 Digit");
    }
}