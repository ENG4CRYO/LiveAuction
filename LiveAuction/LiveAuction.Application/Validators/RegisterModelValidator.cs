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
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email Is Required")
                .EmailAddress().WithMessage("Invalid Email Format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password Is Required")
                .MinimumLength(6).WithMessage("The Password Must Be 6 Char Long As Minimum")
                .Matches("[A-Z]").WithMessage("Must Be Conatain Capital Letter")
                .Matches("[a-z]").WithMessage("Must Be Conatain Small Letter")
                .Matches("[0-9]").WithMessage("Must Be Conatain Number")
                .Matches(@"[\W_]").WithMessage("Must Be Contain A Special Character");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Name Is Required")
                .MaximumLength(50).WithMessage("Name Is Too Long");
        }
    }
}
