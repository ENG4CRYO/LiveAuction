using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using LiveAuction.Application.Dtos.AuthModel;

namespace LiveAuction.Application.Validators
{
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        public RegisterModelValidator()
        {
            RuleFor(x => x.RegisterToken)
                .NotEmpty().WithMessage("Register Token is required. Please verify your email first.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password Is Required")
                .MinimumLength(6).WithMessage("Password Must Be 6 Char Long As Minimum")
                .Matches("[A-Z]").WithMessage("Password Must Be Conatain Capital Letter")
                .Matches("[a-z]").WithMessage("Password Must Be Conatain Small Letter")
                .Matches("[0-9]").WithMessage("Password Must Be Conatain Number")
                .Matches(@"[\W_]").WithMessage("Password Must Be Contain A Special Character");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name Is Required")
                .MaximumLength(50).WithMessage("Name Is Too Long");

            RuleFor(x => x.LastName)
               .NotEmpty().WithMessage("Last Name Is Required")
               .MaximumLength(50).WithMessage("Name Is Too Long");
        }
    }
}
