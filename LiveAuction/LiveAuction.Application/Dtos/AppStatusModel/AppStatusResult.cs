using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Dtos.AppStatusModel
{
    public class AppStatusResult
    {
        public bool IsMaintenance { get; set; }
        public bool UpdateRequired { get; set; }
        public bool IsBanned { get; set; } 
        public string Message { get; set; } = string.Empty;
        public string StoreUrl { get; set; } = string.Empty;
    }
}
