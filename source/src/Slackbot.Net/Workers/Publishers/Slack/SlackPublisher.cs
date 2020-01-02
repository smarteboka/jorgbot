using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.SlackClients;

namespace Slackbot.Net.Workers.Publishers.Slack
{
    public class SlackPublisher : IPublisher
    {
        private readonly ISlackClient _sender;
        private readonly ILogger<SlackPublisher> _logger;

        public SlackPublisher(ISlackClient sender, ILogger<SlackPublisher> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Publish(Notification notification)
        {
            _logger.LogInformation(notification.Msg);
            await _sender.ChatPostMessage(notification.Recipient, notification.Msg);
        }
    }
}