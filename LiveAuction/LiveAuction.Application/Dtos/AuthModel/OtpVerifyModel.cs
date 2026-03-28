using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Dtos.AuthModel
{
    public class OtpVerifyModel
    {
        public string Email { get; set; } = string.Empty;
        public int? Otp { get; set; } 
    }
}
