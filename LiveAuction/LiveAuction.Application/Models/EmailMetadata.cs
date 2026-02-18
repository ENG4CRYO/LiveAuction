using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Models
{
    public class EmailMetadata
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public EmailMetadata(string to, string subject, string body)
        {
            ToEmail = to;
            Subject = subject;
            Body = body;
        }
    }
}
