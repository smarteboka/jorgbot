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
    public class HappyBirthday : RecurringAction
    {
        private readonly Smartinger _smartinger;
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly Timing _timing;
        private readonly SlackChannels _channels;

        public HappyBirthday(Smartinger smartinger,
            IEnumerable<IPublisher> publishers,
            Timing timing,
            SlackChannels channels,
            ILogger<HappyBirthday> logger,
            IOptionsSnapshot<CronOptions> options)
            : base(options,logger)
        {
            _smartinger = smartinger;
            _publishers = publishers;
            _timing = timing;
            _channels = channels;
        }

        public override async Task Process()
        {
            foreach (var smarting in _smartinger.ThatHasBirthday())
            {
                foreach (var p in _publishers)
                {
                    var notification = new Notification
                    {
                        Msg = $"Idag jazzulerer vi {smarting.Name} med {_timing.CalculateAge(smarting.BirthDate)}-årsdagen!",
                        IconEmoji = ":birthday:",
                        Channel = _channels.SmartebokaChannel
                    };
                    await p.Publish(notification);
                }
            }
        }
    }
}