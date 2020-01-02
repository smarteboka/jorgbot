using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net;
using Slackbot.Net.Abstractions.Publishers;

namespace Smartbot.Utilities.Storsdager.RecurringActions
{
    public class StorsdagMention : RecurringAction
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public StorsdagMention(IEnumerable<IPublisher> publishers,
            SlackChannels channels,
            ILogger<StorsdagMention> logger)
            : base(Crons.LastThursdayOfMonthCron,logger)
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
                    Msg = $"Storsdag!",
                    Recipient = _channels.SmartebokaChannel
                };
                await publisher.Publish(notification);
            }
        }
    }
}