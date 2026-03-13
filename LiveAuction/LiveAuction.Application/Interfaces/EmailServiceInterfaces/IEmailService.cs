using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Interfaces.EmailServiceInterfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
