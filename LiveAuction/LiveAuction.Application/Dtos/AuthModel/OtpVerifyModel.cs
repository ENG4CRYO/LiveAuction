using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Dtos.AuthModel
{
    public class OtpVerifyModel
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}
