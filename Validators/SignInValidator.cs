using FluentValidation;
using SyncSharpServer.Models.RequestModels;

namespace SyncSharpServer.Validators
{
    public class SignInValidator : AbstractValidator<SignInRequestModel>
    {
        public SignInValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .Matches(@"^[a-zA-Z][a-zA-Z0-9_.-]*$").WithMessage("UserName must start with a letter and can only contain letters, numbers, underscores, dots, and hyphens.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}
    