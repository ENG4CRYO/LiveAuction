using FluentValidation;
using LiveAuction.Application.Dtos.AuthModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Validators.AuthValidator
{
    public class OtpRequestModelValidator : AbstractValidator<OtpRequestModel>
    {
        public OtpRequestModelValidator() 
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email Is Required")
                .EmailAddress().WithMessage("Invalid Email Format")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Invalid Email Format");
        }
    }
}
