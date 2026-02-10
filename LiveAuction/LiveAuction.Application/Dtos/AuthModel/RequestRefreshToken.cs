using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Dtos.AuthModel
{
    public class RequestRefreshToken
    {
        public string Token { get; set; } = default!;
    }
}
