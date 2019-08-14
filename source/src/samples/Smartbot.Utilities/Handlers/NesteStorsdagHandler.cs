using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net;
using Slackbot.Net.Workers;
using Slackbot.Net.Workers.Handlers;
using Slackbot.Net.Workers.Publishers;
using SlackConnector.Models;
using Smartbot.Utilities.RecurringActions;

namespace Smartbot.Utilities.Handlers
{
    public class NesteStorsdagHandler : IHandleMessages
    {
        private readonly IEnumerable<IPublisher> _publishers;


        public NesteStorsdagHandler(IEnumerable<IPublisher> publishers)
        {
            _publishers = publishers;
        }

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var upcomingEvents = Timing.GetNextOccurences(Storsdag.LastThursdayOfMonthCron);
            var nextStorsdag = upcomingEvents.FirstOrDefault();
            var culture = new CultureInfo("nb-NO");
            foreach (var publisher in _publishers)
            {
                var notification = new Notification
                {
                    Msg = $"Neste storsdag er {nextStorsdag.Date.ToString(culture.DateTimeFormat.ShortDatePattern, culture)}",
                    Recipient = message.ChatHub.Id
                };
                await publisher.Publish(notification);
            }
            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            var containsNeste = message.Text.Contains("neste", StringComparison.InvariantCultureIgnoreCase);
            var containsStorsdag = message.Text.Contains("storsdag", StringComparison.InvariantCultureIgnoreCase);
            return message.MentionsBot && containsNeste && containsStorsdag;
        }
    }
}