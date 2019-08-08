using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net;
using Slackbot.Net.Hosting;
using Slackbot.Net.Publishers;
using Slackbot.Net.Publishers.Slack;

namespace Smartbot.Utilities.RecurringActions
{
    public class Storsdag : RecurringAction
    {
        public const string LastThursdayOfMonthCron = "0 0 8 * * THUL";
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public Storsdag(IEnumerable<IPublisher> publishers,
            SlackChannels channels,
            ILogger<Storsdag> logger, IOptionsSnapshot<CronOptions> options)
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
                    Msg = $"Storsdags inc! Med? :+1: / :-1: ?",
                    IconEmoji = ":beer:",
                    Channel = _channels.SmartebokaChannel
                };
                await publisher.Publish(notification);
            }
        }
    }
}