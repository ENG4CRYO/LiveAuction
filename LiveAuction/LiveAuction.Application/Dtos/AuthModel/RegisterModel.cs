using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LiveAuction.Application.Dtos.AuthModel
{
    public class RegisterModel
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string RegisterToken { get; set; } = default!;

    }
}
