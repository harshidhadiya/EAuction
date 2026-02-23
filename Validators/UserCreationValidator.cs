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
                return ;
            });
            RuleFor(x => x.password).NotEmpty().WithMessage("password must filled up");
            RuleFor(x=>x.role).NotEmpty().When(x=>{

                if (x.role!="USER" || x.role!="ADMIN")
                {
                    return true;
                }
                return false;
            }).WithMessage("either role should be user or admin");
        }
    }
}