using System;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Endpoints.Abstractions;
using Slackbot.Net.Endpoints.Models.Events;
using Slackbot.Net.SlackClients.Http;
using Smartbot.Data.Storage.Events;

namespace Smartbot.Utilities.Handlers
{
    public class NesteStorsdagHandler : IHandleAppMentions
    {
        private readonly ISlackClient _client;
        private readonly IEventsStorage _eventStorage;
        private readonly IInvitationsStorage _inviteStorage;


        public NesteStorsdagHandler(ISlackClient client, IEventsStorage eventStorage, IInvitationsStorage inviteStorage)
        {
            _client = client;
            _eventStorage = eventStorage;
            _inviteStorage = inviteStorage;
        }

        public (string, string) GetHelpDescription() => ("neste storsdag", "Viser litt info om neste storsdag");

        public async Task<EventHandledResponse> Handle(EventMetaData data, AppMentionEvent message)
        {
            var nextStorsdag = await _eventStorage.GetNextEvent(EventTypes.StorsdagEventType);
            var invitations = await _inviteStorage.GetInvitations(nextStorsdag.RowKey);

            var rapport = nextStorsdag.Topic;
            foreach (var inviteGroup in invitations.GroupBy(i => i.Rsvp))
            {
                string names = inviteGroup.Select(i => i.SlackUsername).Aggregate((x, y) => x + ", " + y);
                rapport += $"\n• `{inviteGroup.Key}` : {names}";
            }

            await _client.ChatPostMessage(message.Channel, rapport);
            return new EventHandledResponse("OK");
        }

        public bool ShouldHandle(AppMentionEvent message) => message.Text.StartsWith("cmd") && message.Text.Contains("neste", StringComparison.InvariantCultureIgnoreCase) && message.Text.Contains("storsdag", StringComparison.InvariantCultureIgnoreCase);
    }
}
