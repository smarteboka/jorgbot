using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net;
using Slackbot.Net.Abstractions.Publishers;

namespace Smartbot.Utilities.RecurringActions
{
    public class HeartBeater : RecurringAction
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public HeartBeater(IEnumerable<IPublisher> publishers,
            SlackChannels channels,
            ILogger<HeartBeater> logger)
            : base(Crons.EveryDayAtSeven55, logger)
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
                    Msg = $":heart:",
                    Recipient = _channels.TestChannel
                };
                await publisher.Publish(notification);
            }
        }
    }
}