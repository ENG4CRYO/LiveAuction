using LiveAuction.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Infrastructure.Services.Background
{
    public class EmailBackgroundWorker : BackgroundService
    {
        private readonly IEmailQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailBackgroundWorker> _logger;

        public EmailBackgroundWorker(
            IEmailQueue queue,
            IServiceProvider serviceProvider,
            ILogger<EmailBackgroundWorker> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Background Worker Started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    var emailMetadata = await _queue.DequeueAsync(stoppingToken);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                        await emailService.SendEmailAsync(
                            emailMetadata.ToEmail,
                            emailMetadata.Subject,
                            emailMetadata.Body
                        );

                        _logger.LogInformation($"Email sent to {emailMetadata.ToEmail} successfully.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while sending email in background.");
                }
            }
        }
    }
}
