using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Slackbot.Net.Workers.Publishers.Logger
{
    public class LoggerPublisher : IPublisher
    {
        private readonly ILogger<LoggerPublisher> _logger;

        public LoggerPublisher(ILogger<LoggerPublisher> logger)
        {
            _logger = logger;
        }

        public Task Publish(Notification notification)
        {
            _logger.LogInformation($"[{DateTime.UtcNow.ToLongTimeString()}] {notification.Msg}");
            return Task.CompletedTask;
        }
    }
}