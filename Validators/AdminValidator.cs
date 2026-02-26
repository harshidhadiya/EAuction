using System.Data;
using FluentValidation;
using MACUTION.Model.Dto;

namespace MACUTION.Validators
{
    public class signupValidator:AbstractValidator<signupRequestDto> 
    {
        public signupValidator()
        {
            RuleFor(x=>x.Name).NotEmpty().WithMessage("Your Name Cannot be empty");
            RuleFor(x=>x.Email).NotEmpty().WithMessage("Your Email should");
            RuleFor(x=>x.MobileNumber).NotEmpty().WithMessage("You Have To add contact detail for the verication of the Your Product");
            RuleFor(x=>x.role).Must(x=>x=="ADMIN").WithMessage("Sorry But this features for sign up only for the admin people");
            RuleFor(x=>x.Address).NotEmpty().WithMessage("You Have To address because of on verification time we need the address");
            RuleFor(x=>x.Password).NotEmpty().WithMessage("Withou Password How we can authenticate this thing's tell me");
        }
    }
}