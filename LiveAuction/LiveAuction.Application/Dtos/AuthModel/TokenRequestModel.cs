using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace LiveAuction.Application.Dtos.AuthModel
{
    public class TokenRequestModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
