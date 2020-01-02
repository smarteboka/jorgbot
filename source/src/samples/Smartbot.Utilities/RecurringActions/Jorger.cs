using System.Collections.Generic;
using System.Threading.Tasks;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Publishers;

namespace Smartbot.Utilities.RecurringActions
{
    public class Jorger : IRecurringAction
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public Jorger(IEnumerable<IPublisher> publishers, SlackChannels channels)
        {
            _publishers = publishers;
            _channels = channels;
        }

        public async Task Process()
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

        public string Cron => Crons.EveryDayAtNine;
    }
}