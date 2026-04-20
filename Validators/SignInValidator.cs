using FluentValidation;
using SyncSharpServer.Models.RequestModels;

namespace SyncSharpServer.Validators
{
    public class SignInValidator : AbstractValidator<SignInRequestModel>
    {
        public SignInValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}
