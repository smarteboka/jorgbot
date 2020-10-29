using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Endpoints.Abstractions;
using Slackbot.Net.Endpoints.Models.Events;
using Slackbot.Net.SlackClients.Http;
using Smartbot.Utilities.Storsdager.RecurringActions;

namespace Smartbot.Utilities.Handlers
{
    public class RsvpReminder : IHandleAppMentions
    {
        private readonly StorsdagInviter _inviter;
        private readonly ISlackClient _client;

        public RsvpReminder(StorsdagInviter inviter, ISlackClient client)
        {
            _inviter = inviter;
            _client = client;
        }


        public (string, string) GetHelpDescription() => ("storsdagreminder","Reminds smartinger to answer storsdag");

        public async Task<EventHandledResponse> Handle(EventMetaData data, AppMentionEvent message)
        {
            var unanswered = await _inviter.SendRemindersToUnanswered();
            if (unanswered.Any())
            {
                var uText = unanswered.Select(u => $"Reminded {u.SlackUsername} to RSVP {u.EventTopic}");
                var aggr = $"{uText.Aggregate((x, y) => x + "\n" + y)}";
                await _client.ChatPostMessage(message.Channel, aggr);
            }
            else
            {
                await _client.ChatPostMessage(message.Channel, $"No unanswered invites or no existing invites for next storsdag");
            }

            return new EventHandledResponse("OK");
        }

        public bool ShouldHandle(AppMentionEvent message) => message.User == "johnkors" && message.Text.Contains("storsdagreminder");
    }
}