using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions;

namespace Slackbot.Net.Workers.Publishers.Slack
{
    public class SlackPublisher : IPublisher
    {
        private readonly SlackTaskClientExtensions _sender;
        private readonly ILogger<SlackPublisher> _logger;

        public SlackPublisher(SlackTaskClientExtensions sender, ILogger<SlackPublisher> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Publish(Notification notification)
        {
            _logger.LogInformation(notification.Msg);
            await _sender.PostMessageAsync(notification.Recipient,notification.Msg);
        }
    }
}