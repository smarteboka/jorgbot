using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Smartbot.Publishers;
using Smartbot.Publishers.Slack;

namespace Smartbot.HostedServices.CronServices
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

        protected override string Cron()
        {
            return LastThursdayOfMonthCron; 
        }

        protected override async Task Process()
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