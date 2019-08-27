using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net.Workers;
using Slackbot.Net.Workers.Configuration;
using Slackbot.Net.Workers.Publishers;

namespace Smartbot.Utilities.Storsdager.RecurringActions
{
    public class StorsdagReminder : RecurringAction
    {
        public const string LastThursdayOfMonthCron = "0 0 8 * * THUL";
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public StorsdagReminder(IEnumerable<IPublisher> publishers,
            SlackChannels channels,
            ILogger<StorsdagReminder> logger, IOptionsSnapshot<CronOptions> options)
            : base(options,logger)
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