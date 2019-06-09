using System.Collections.Generic;
using System.Threading.Tasks;
using JorgBot.Publishers;
using JorgBot.Publishers.Slack;
using Microsoft.Extensions.Logging;

namespace JorgBot.HostedServices.CronServices
{
    public class JorgingHostedService : CronHostedService
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public JorgingHostedService(IEnumerable<IPublisher> publishers, SlackChannels channels, ILogger<JorgingHostedService> logger)
            : base(logger)
        {
            _publishers = publishers;
            _channels = channels;
        }

        protected override string Cron()
        {
            return "0 55 7 * * *"; 
        }

        protected override async Task Process()
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