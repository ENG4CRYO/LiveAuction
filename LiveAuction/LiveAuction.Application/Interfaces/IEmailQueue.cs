using LiveAuction.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Interfaces
{
    public interface IEmailQueue
    {
        ValueTask QueueBackgroundEmailAsync(EmailMetadata email);
        ValueTask<EmailMetadata> DequeueAsync(CancellationToken cancellationToken);
    }
}
