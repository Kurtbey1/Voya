using FluentValidation;
using Voya.Dtos.Auth;

namespace Voya.Services.Auth.Validator
{
    public class SignupRequestValidator:AbstractValidator<SignupReqDto>
    {
        public SignupRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is mandatory.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("User Name is required.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.");

            RuleFor(x => x.Nationality)
                .NotEmpty().WithMessage("Nationality is required.");
        }
    }
}
