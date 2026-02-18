using LiveAuction.Application.Interfaces;
using LiveAuction.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace LiveAuction.Infrastructure.Services.Background
{
    public class EmailQueue : IEmailQueue
    {
        private readonly Channel<EmailMetadata> _queue;
        public EmailQueue()
        {
            var options = new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<EmailMetadata>(options);
        }

        public async ValueTask QueueBackgroundEmailAsync(EmailMetadata email)
        {
            if (email == null) return;
            await _queue.Writer.WriteAsync(email);
        }

        public async ValueTask<EmailMetadata> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
