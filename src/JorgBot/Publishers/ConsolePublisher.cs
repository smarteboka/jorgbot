using System.Threading.Tasks;
using JorgBot.HostedServices;
using Microsoft.Extensions.Logging;

namespace JorgBot.Publishers
{
    internal class ConsolePublisher : IPublisher
    {
        private readonly ILogger<ConsolePublisher> _logger;

        public ConsolePublisher(ILogger<ConsolePublisher> logger)
        {
            _logger = logger;
        }

        public Task Publish(Notification notification)
        {
            _logger.LogInformation($"[{Timing.NowUtc().ToLongTimeString()}] {notification.Msg}");
            return Task.CompletedTask;
        }
    }
}