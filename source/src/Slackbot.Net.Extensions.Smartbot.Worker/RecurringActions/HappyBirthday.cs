using System.Collections.Generic;
using System.Threading.Tasks;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Publishers;
using Smartbot.Utilities.Times;

namespace Smartbot.Utilities.RecurringActions
{
    public class HappyBirthday : IRecurringAction
    {
        private readonly Smartinger _smartinger;
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;
        private readonly Timing _timing;

        public HappyBirthday(Smartinger smartinger, IEnumerable<IPublisher> publishers, SlackChannels channels)
        {
            _smartinger = smartinger;
            _publishers = publishers;
            _channels = channels;
            _timing = new Timing();
        }

        public async Task Process()
        {
            foreach (var smarting in _smartinger.ThatHasBirthday())
            {
                foreach (var p in _publishers)
                {
                    var notification = new Notification
                    {
                        Msg = $"Idag jazzulerer vi {smarting.Name} med {_timing.CalculateAge(smarting.BirthDate)}-årsdagen!",
                        Recipient = _channels.SmartebokaChannel
                    };
                    await p.Publish(notification);
                }
            }
        }

        public string Cron => Crons.EveryDayAtEight;
    }
}