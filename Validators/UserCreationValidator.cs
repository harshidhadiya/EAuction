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
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email format is not valid");

            RuleFor(x => x.MobileNumber)
                .NotEmpty().WithMessage("Mobile number is required");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required");

            RuleFor(x => x.Password).NotEmpty().WithMessage("password must filled up");
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

    // Simple validator for login DTO.
    public class UserLoginValidator : AbstractValidator<UserLoginDto>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email format is not valid");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password must not be empty");

            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(role => role == "USER" || role == "ADMIN")
                .WithMessage("Role must be either USER or ADMIN");
        }
    }
}