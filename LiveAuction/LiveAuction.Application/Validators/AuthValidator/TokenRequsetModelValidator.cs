using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using LiveAuction.Application.Dtos.AuthModel;

namespace LiveAuction.Application.Validators.AuthValidator
{
    public class TokenRequsetModelValidator : AbstractValidator<TokenRequestModel>
    {
        public TokenRequsetModelValidator()
        {
            RuleFor(x => x.Email)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("Email Is Required")
                 .EmailAddress().WithMessage("Invalid Email Format")
                 .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Invalid Email Format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password Is Required");

        }
    }
}
