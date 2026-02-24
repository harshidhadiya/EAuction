using FluentValidation;
using MACUTION.Model.Dto;

namespace MACUTION.Validators

{
    public class productCreationValidator : AbstractValidator<productDto>
    {
        public productCreationValidator()
        {
            RuleFor(x=>x.Name).NotEmpty().WithMessage("Sorry Without Name we are not accepting the productname");
            RuleFor(x=>x.date).NotEmpty().WithMessage("without date we are not able to verified your it's okay to be add approximate date");
            
        }
    }
}