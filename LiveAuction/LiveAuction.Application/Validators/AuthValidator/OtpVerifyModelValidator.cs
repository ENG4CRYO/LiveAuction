using FluentValidation;
using LiveAuction.Application.Dtos.AuthModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Validators.AuthValidator
{
    public class OtpVerifyModelValidator : AbstractValidator<OtpVerifyModel>
    {
        public OtpVerifyModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email Is Required")
                .EmailAddress().WithMessage("Invalid Email Format");

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP Is Required");
        }
    }
}
