using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Publishers;
using SlackConnector.Models;
using Smartbot.Utilities.Storage.Events;

namespace Smartbot.Utilities.Handlers
{
    public class NesteStorsdagHandler : IHandleMessages
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly EventsStorage _eventStorage;
        private readonly InvitationsStorage _inviteStorage;


        public NesteStorsdagHandler(IEnumerable<IPublisher> publishers, EventsStorage eventStorage, InvitationsStorage inviteStorage)
        {
            _publishers = publishers;
            _eventStorage = eventStorage;
            _inviteStorage = inviteStorage;
        }

        public bool ShouldShowInHelp => true;


        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("neste storsdag", "Viser litt info om neste storsdag");

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var nextStorsdag = await _eventStorage.GetNextEvent(EventTypes.StorsdagEventType);
            var invitations = await _inviteStorage.GetInvitations(nextStorsdag.RowKey);

            var rapport = nextStorsdag.Topic;
            foreach (var inviteGroup in invitations.GroupBy(i => i.Rsvp))
            {
                string names = inviteGroup.Select(i => i.SlackUsername).Aggregate((x, y) => x + ", " + y);
                rapport += $"\n• `{inviteGroup.Key}` : {names}";
            }

            var culture = new CultureInfo("nb-NO");
            foreach (var publisher in _publishers)
            {
                var notification = new Notification
                {
                    Msg = rapport,
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