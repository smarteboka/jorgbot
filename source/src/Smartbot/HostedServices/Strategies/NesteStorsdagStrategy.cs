using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SlackConnector.Models;
using Smartbot.HostedServices.CronServices;
using Smartbot.Publishers;

namespace Smartbot.HostedServices.Strategies
{
    public class NesteStorsdagStrategy : IReplyStrategy
    {
        private readonly IEnumerable<IPublisher> _publishers;

        public NesteStorsdagStrategy(IEnumerable<IPublisher> publishers)
        {
            _publishers = publishers;
        }
        
        public async Task Handle(SlackMessage message)
        {
            var upcomingEvents = Timing.GetNextOccurences(StorsdagsWeekHostedService.LastThursdayOfMonthCron);
            var nextStorsdag = upcomingEvents.FirstOrDefault();
            var culture = new CultureInfo("nb-NO"); 
            foreach (var publisher in _publishers)
            {
                var notification = new Notification
                {
                    Msg = $"Neste storsdag er {nextStorsdag.Date.ToString(culture.DateTimeFormat.ShortDatePattern, culture)}", 
                    Channel = message.ChatHub.Id
                };
                await publisher.Publish(notification);
            }
        }

        public bool ShouldExecute(SlackMessage message)
        {
            var containsNeste = message.Text.Contains("neste", StringComparison.InvariantCultureIgnoreCase);
            var containsStorsdag = message.Text.Contains("storsdag", StringComparison.InvariantCultureIgnoreCase);
            return message.MentionsBot && containsNeste && containsStorsdag;
        }
    }
}