using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net;
using Slackbot.Net.Publishers;
using Slackbot.Net.Strategies;
using SlackConnector.Models;
using Smartbot.Utilities.HostedServices;

namespace Smartbot.Utilities.Strategies
{
    public class StorsdagerHandler : IHandleMessages
    {
        private readonly IEnumerable<IPublisher> _publishers;

        public StorsdagerHandler(IEnumerable<IPublisher> publishers)
        {
            _publishers = publishers;
        }

        public async Task<HandleResponse> Handle(SlackMessage message)
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
            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            var containsStorsdager = message.Text.Contains("storsdager", StringComparison.InvariantCultureIgnoreCase);
            return message.MentionsBot && containsStorsdager;
        }
    }
}