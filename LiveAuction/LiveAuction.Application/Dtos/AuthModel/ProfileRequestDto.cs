using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Dtos.AuthModel
{
    public class ProfileRequestDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public int SoldItems { get; set; }
        public int TotalItems { get; set; }
        public decimal AvgPrice { get; set; }  
    }
}
