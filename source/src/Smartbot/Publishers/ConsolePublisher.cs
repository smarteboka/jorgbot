using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Smartbot.Publishers
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
            _logger.LogInformation($"[{Timing.NowInOsloTime().DateTime.ToLongTimeString()}] {notification.Msg}");
            return Task.CompletedTask;
        }
    }
}