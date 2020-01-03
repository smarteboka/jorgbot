using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Publishers;
using Smartbot.Utilities.Storsdager.RecurringActions;

namespace Smartbot.Utilities.Handlers
{
    public class RsvpReminder : IHandleMessages
    {
        private readonly StorsdagInviter _inviter;
        private readonly IEnumerable<IPublisher> _publishers;

        public RsvpReminder(StorsdagInviter inviter, IEnumerable<IPublisher> publishers)
        {
            _inviter = inviter;
            _publishers = publishers;
        }

        public bool ShouldShowInHelp => false;

        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("storsdagreminder","Reminds smartinger to answer storsdag");

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var unanswered = await _inviter.SendRemindersToUnanswered();
            if (unanswered.Any())
            {
                var uText = unanswered.Select(u => $"Reminded {u.SlackUsername} to RSVP {u.EventTopic}");
                var aggr = $"{uText.Aggregate((x, y) => x + "\n" + y)}";
                foreach (var publisher in _publishers)
                {
                    var notification = new Notification {Msg = aggr, Recipient = message.ChatHub.Id};
                    await publisher.Publish(notification);
                }
            }
            else
            {
                foreach (var publisher in _publishers)
                {
                    var notification = new Notification
                    {
                        Msg = $"No unanswered invites or no existing invites for next storsdag",
                        Recipient = message.ChatHub.Id
                    };
                    await publisher.Publish(notification);
                }
            }

            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            return message.ChatHub.Type == ChatHubTypes.DirectMessage && message.User.Name == "johnkors" && message.Text == "storsdagreminder";
        }
    }
}