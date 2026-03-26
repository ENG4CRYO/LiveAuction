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
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("Email Is Required")
                 .EmailAddress().WithMessage("Invalid Email Format")
                 .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Invalid Email Format");

            RuleFor(x => x.Otp)
                .Cascade(cascadeMode: CascadeMode.Stop)
                .NotEmpty().WithMessage("OTP Is Required")
                .Length(6).WithMessage("OTP Must be 6 number")
                .Matches(@"^\d+$").WithMessage("OTP Must contain numbers only");
        }
    }
}
