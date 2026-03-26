using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using LiveAuction.Application.Dtos.AuthModel;

namespace LiveAuction.Application.Validators.AuthValidator
{
    public class RequestRefreshTokenValidator : AbstractValidator<RequestRefreshToken>
    {
        public RequestRefreshTokenValidator()
        {
            RuleFor(x => x.Token)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Refresh Token Cannot Be Null")
                .NotEmpty().WithMessage("Refresh Token Canneot Be Empty");
        }
    }
}
