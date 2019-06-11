using System.Collections.Generic;
using System.Threading.Tasks;
using JorgBot.Publishers;
using JorgBot.Publishers.Slack;
using Microsoft.Extensions.Logging;

namespace JorgBot.HostedServices.CronServices
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

        protected override string Cron()
        {
            return "0 5 21 * * *"; 
        }

        protected override async Task Process()
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