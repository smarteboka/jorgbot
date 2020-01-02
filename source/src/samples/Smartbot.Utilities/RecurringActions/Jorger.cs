using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net;
using Slackbot.Net.Abstractions.Publishers;

namespace Smartbot.Utilities.RecurringActions
{
    public class Jorger : RecurringAction
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public Jorger(IEnumerable<IPublisher> publishers,
            SlackChannels channels,
            ILogger<Jorger> logger)
            : base(Crons.EveryDayAtNine,logger)
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
                    Recipient = _channels.JorgChannel
                };
                await publisher.Publish(notification);
            }
        }
    }
}