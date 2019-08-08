using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net;
using Slackbot.Net.Publishers;
using Slackbot.Net.Publishers.Slack;

namespace Smartbot.Utilities.HostedServices
{
    public class StorsdagsWeekHostedService : CronHostedService
    {
        public const string LastThursdayOfMonthCron = "0 0 8 * * THUL";
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public StorsdagsWeekHostedService(IEnumerable<IPublisher> publishers, SlackChannels channels, ILogger<StorsdagsWeekHostedService> logger)
            : base(logger)
        {
            _publishers = publishers;
            _channels = channels;
        }

        public override string Cron()
        {
            return LastThursdayOfMonthCron; 
        }

        public override async Task Process()
        {
            foreach (var publisher in _publishers)
            {
                var notification = new Notification
                {
                    Msg = $"Storsdags inc! Med? :+1: / :-1: ?", 
                    IconEmoji = ":beer:", 
                    Channel = _channels.SmartebokaChannel
                };
                await publisher.Publish(notification);
            }
        }
    }
}