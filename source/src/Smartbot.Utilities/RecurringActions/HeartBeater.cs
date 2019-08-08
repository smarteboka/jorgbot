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
    public class HeartBeater : RecurringAction
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;
        private string _cron;

        public HeartBeater(IEnumerable<IPublisher> publishers,
            SlackChannels channels,
            ILogger<HeartBeater> logger, IOptionsSnapshot<CronOptions> options)
            : base(options, logger)
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
                    BotName = "heartbot",
                    IconEmoji = ":heart:",
                    Channel = _channels.TestChannel
                };
                await publisher.Publish(notification);
            }
        }
    }
}