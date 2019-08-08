using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net;
using Slackbot.Net.Hosting;
using Slackbot.Net.Publishers;
using Slackbot.Net.Publishers.Slack;

namespace Smartbot.Utilities.HostedServices
{
    public class JorgingHostedService : RecurringAction
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public JorgingHostedService(IEnumerable<IPublisher> publishers,
            SlackChannels channels,
            ILogger<JorgingHostedService> logger,
            IOptionsSnapshot<CronOptions> options)
            : base(options,logger)
        {
            _publishers = publishers;
            _channels = channels;
        }

        public override async Task Process()
        {
            foreach (var publisher in _publishers)
            {
                var notification = new Notification
                {
                    Msg = $"jorg",
                    BotName = "jorgbot",
                    IconEmoji = ":coffee:",
                    Channel = _channels.JorgChannel
                };
                await publisher.Publish(notification);
            }
        }
    }
}