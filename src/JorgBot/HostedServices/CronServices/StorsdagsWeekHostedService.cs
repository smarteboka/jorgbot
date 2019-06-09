using System.Collections.Generic;
using System.Threading.Tasks;
using JorgBot.Publishers;
using JorgBot.Publishers.Slack;
using Microsoft.Extensions.Logging;

namespace JorgBot.HostedServices.CronServices
{
    public class StorsdagsWeekHostedService : CronHostedService
    {
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
            return "0 0 8 * * THUL"; 
        }

        protected override async Task Process()
        {
            foreach (var publisher in _publishers)
            {
                var notification = new Notification
                {
                    Msg = $"Storsdags inc! Med? :+1: / :-1: ?", 
                    BotName = "storsdagsbot", 
                    IconEmoji = ":beer:", 
                    Channel = _channels.StorsdagChannel
                };
                await publisher.Publish(notification);
            }
        }
    }
}