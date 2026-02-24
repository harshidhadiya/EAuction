using System.Data;
using System.Text.RegularExpressions;
using FluentValidation;
using MACUTION.Model.Dto;

namespace MACUTION.Validators
{
    public class UserCreationValidators : AbstractValidator<UserCreation>
    {
        public UserCreationValidators()
        {
            RuleFor(x => x.Name).Custom((name, context) =>
            {

                if (String.IsNullOrEmpty(name))
                {
                    context.AddFailure("Name", "Empty CanNot Be Accepted Here");
                    return;
                }
                if (name.Length < 3)
                {
                    context.AddFailure("Lenght Must contain more then 3 lenght ");
                }
                return;
            });
            RuleFor(x => x.password).NotEmpty().WithMessage("password must filled up");
            RuleFor(x => x.role)
                                .NotEmpty()
                                .Must(role => role == "USER" || role == "ADMIN")
                                .WithMessage("Role must be either USER or ADMIN");
        }
    }
    public class changePasswordValidators : AbstractValidator<changePasswordDto>
    {
        public changePasswordValidators()
        {
         RuleFor(x=>x.password).NotEmpty().WithMessage("password should not be empty okay");
         RuleFor(x=>x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password Couldn't Empty");
         RuleFor(x=>x.password).Equal(y=>y.ConfirmPassword).WithErrorCode("401").WithMessage("Your Confirm Password couldn't match with the Password Field");
        }
    }
}