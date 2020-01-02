using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net;
using Slackbot.Net.Abstractions.Publishers;

namespace Smartbot.Utilities.RecurringActions
{
    public class HappyBirthday : RecurringAction
    {
        private readonly Smartinger _smartinger;
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public HappyBirthday(Smartinger smartinger,
            IEnumerable<IPublisher> publishers,
            SlackChannels channels,
            ILogger<HappyBirthday> logger
            )
            : base(Crons.EveryDayAtEight,logger)
        {
            _smartinger = smartinger;
            _publishers = publishers;
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
                        Msg = $"Idag jazzulerer vi {smarting.Name} med {Timing.CalculateAge(smarting.BirthDate)}-årsdagen!",
                        Recipient = _channels.SmartebokaChannel
                    };
                    await p.Publish(notification);
                }
            }
        }
    }
}