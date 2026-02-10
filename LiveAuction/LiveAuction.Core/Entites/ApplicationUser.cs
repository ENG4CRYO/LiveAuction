using System;
using System.Collections.Generic;
using System.Text;
using LiveAuction.Core.Entites.AuthEntites;
using Microsoft.AspNetCore.Identity;

namespace LiveAuction.Core.Entites
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
