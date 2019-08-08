using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net;
using Slackbot.Net.Publishers;
using Slackbot.Net.Publishers.Slack;

namespace Smartbot.Utilities.HostedServices
{
    public class HeartBeatHostedService : CronHostedService
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public HeartBeatHostedService(IEnumerable<IPublisher> publishers, SlackChannels channels, ILogger<HeartBeatHostedService> logger)
            : base(logger)
        {
            _publishers = publishers;
            _channels = channels;
        }

        public override string Cron()
        {
            return "0 0 8 * * *";
        }

        public override async Task Process()
        {
            foreach (var publisher in _publishers)
            {
                var notification = new Notification
                {
                    Msg = $":heart:",
                    BotName = "heartbot",
                    IconEmoji = ":heart:",
                    Channel = _channels.TestChannel
                };
                await publisher.Publish(notification);
            }
        }
    }
}