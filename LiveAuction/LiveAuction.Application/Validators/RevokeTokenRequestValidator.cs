using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using LiveAuction.Application.Dtos.AuthModel;

namespace LiveAuction.Application.Validators
{
    public class RevokeTokenRequestValidator : AbstractValidator<RevokeTokenRequest>
    {
        public RevokeTokenRequestValidator()
        {
            RuleFor(x => x.Token).NotNull().WithMessage("Token Request Cannot Be Null")
                .NotEmpty().WithMessage("Token Request Canneot Be Empty");
        }
    }
}
