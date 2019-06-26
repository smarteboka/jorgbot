using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Smartbot.Publishers;
using Smartbot.Publishers.Slack;

namespace Smartbot.HostedServices.CronServices
{
    public class BirthdayCheckerHostedService : CronHostedService
    {
        private readonly Smartinger _smartinger;
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly Timing _timing;
        private readonly SlackChannels _channels;

        public BirthdayCheckerHostedService(Smartinger smartinger, IEnumerable<IPublisher> publishers, Timing timing, SlackChannels channels, ILogger<BirthdayCheckerHostedService> logger)
            : base(logger)
        {
            _smartinger = smartinger;
            _publishers = publishers;
            _timing = timing;
            _channels = channels;
        }

        protected override string Cron()
        {
            return "0 0 8 * * *";
        }

        protected override async Task Process()
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