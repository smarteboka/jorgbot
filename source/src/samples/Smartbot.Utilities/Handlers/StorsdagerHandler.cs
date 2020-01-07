using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Handlers.Models.Rtm.MessageReceived;
using Slackbot.Net.Abstractions.Publishers;
using Slackbot.Net.Utilities;

namespace Smartbot.Utilities.Handlers
{
    public class StorsdagerHandler : IHandleMessages
    {
        private readonly IEnumerable<IPublisher> _publishers;

        public StorsdagerHandler(IEnumerable<IPublisher> publishers)
        {
            _publishers = publishers;
        }

        public bool ShouldShowInHelp => true;


        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("storsdager", "Viser kommende storsdager");

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var upcomingEvents = Timing.GetNextOccurences(Crons.LastThursdayOfMonthCron);
            var culture = new CultureInfo("nb-NO");
            var uText = upcomingEvents.Select(u => $"{u.Date.ToString(culture.DateTimeFormat.ShortDatePattern, culture)}");
            var aggr = $"{uText.Aggregate((x, y) => x + "\n" + y)}";
            foreach (var publisher in _publishers)
            {
                var notification = new Notification
                {
                    Msg = aggr,
                    Recipient = message.ChatHub.Id
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