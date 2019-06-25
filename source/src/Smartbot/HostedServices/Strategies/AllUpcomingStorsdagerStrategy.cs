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
    public class AllUpcomingStorsdagerStrategy : IReplyStrategy
    {
        private readonly IEnumerable<IPublisher> _publishers;

        public AllUpcomingStorsdagerStrategy(IEnumerable<IPublisher> publishers)
        {
            _publishers = publishers;
        }
        
        public async Task Handle(SlackMessage message)
        {
            var upcomingEvents = Timing.GetNextOccurences(StorsdagsWeekHostedService.LastThursdayOfMonthCron);
            var culture = new CultureInfo("nb-NO");
            var uText = upcomingEvents.Select(u => $"{u.Date.ToString(culture.DateTimeFormat.ShortDatePattern, culture)}");
            var aggr = $"{uText.Aggregate((x, y) => x + "\n" + y)}";
            foreach (var publisher in _publishers)
            {
                var notification = new Notification
                {
                    Msg = aggr, 
                    Channel = message.ChatHub.Id
                };
                await publisher.Publish(notification);
            }
        }

        public bool ShouldExecute(SlackMessage message)
        {
            var containsStorsdager = message.Text.Contains("storsdager", StringComparison.InvariantCultureIgnoreCase);
            return message.MentionsBot && containsStorsdager;
        }
    }
}