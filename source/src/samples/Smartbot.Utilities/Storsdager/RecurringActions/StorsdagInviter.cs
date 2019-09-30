using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SlackAPI;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions.Models;
using Smartbot.Utilities.Storage.Events;

namespace Smartbot.Utilities.Storsdager.RecurringActions
{
    public class StorsdagInviter
    {

        private readonly EventsStorage _eventStorage;
        private readonly InvitationsStorage _inviteStorage;
        private readonly SlackTaskClientExtensions _slackClient;
        private readonly ILogger<StorsdagInviter> _logger;

        public StorsdagInviter(EventsStorage eventStorage, InvitationsStorage inviteStorage, SlackTaskClientExtensions slackClient, ILogger<StorsdagInviter> logger)
        {
            _eventStorage = eventStorage;
            _inviteStorage = inviteStorage;
            _slackClient = slackClient;
            _logger = logger;
        }

        public async Task InviteNextStorsdag()
        {
            var nextStorsdag = await _eventStorage.GetNextEvent(EventTypes.StorsdagEventType);
            if (nextStorsdag != null)
            {
                var invitations = await _inviteStorage.GetInvitations(nextStorsdag.RowKey);
                if (!invitations.Any())
                {
                    var members = await _slackClient.GetUserListAsync();
                    var allSlackUsers = members.members;
                    var filtered = allSlackUsers.Where(u => !u.is_bot && !u.IsSlackBot);
                    //filtered = filtered.Where(u => u.name == "johnkors");
                    foreach (var member in filtered)
                    {
                        await Invite(member, nextStorsdag);
                    }
                }
            }
            else
            {
                _logger.LogWarning($"No events found.");
            }
        }

        public async Task<IEnumerable<InvitationsEntity>> SendRemindersToUnanswered()
        {
            var nextStorsdag = await _eventStorage.GetNextEvent(EventTypes.StorsdagEventType);
            var invitations = await _inviteStorage.GetInvitations(nextStorsdag.RowKey);
            var unanswered = invitations.Where(i => i.Rsvp == RsvpValues.Invited);
            foreach (var invite in unanswered)
            {
                _logger.LogInformation($"Reminding {invite.SlackUsername} about {invite.EventTopic}");
                //var res = await _slackClient.PostMessageAsync(invite.SlackUserId, "Hei, hørte ikke noe fra deg angående storsdag! :/ ", as_user:true);
                //await SendInviteInSlackDM(invite);
            }

            return unanswered;
        }

        private async Task Invite(User user, EventEntity nextStorsdag)
        {
            var invite = new InvitationsEntity(Guid.NewGuid())
            {
                EventId = nextStorsdag.RowKey,
                EventTime = nextStorsdag.EventTime,
                EventTopic = nextStorsdag.Topic,
                SlackUsername = user.name,
                SlackUserId = user.id,
                Rsvp = RsvpValues.Invited
            };
            await _inviteStorage.Save(invite);
            var inviteSentOk = await SendInviteInSlackDM(invite);

            if(inviteSentOk)
            {
                _logger.LogInformation($"Sent invite {nextStorsdag.Topic} to {user.name}");
            }
            else
            {
                _logger.LogError($"Could not send invite {nextStorsdag.Topic} to {user.name}");
            }
        }

        public async Task<bool> SendInviteInSlackDM(InvitationsEntity invite)
        {
            var q = AttendStorsdagQuestion(invite);
            var res = await _slackClient.PostMessageQuestionAsync(q);

            if(!res.ok)
                throw new Exception(res.error);

            return res.ok;
        }

        private Question AttendStorsdagQuestion(InvitationsEntity invitationEntity)
        {
            var q = new Question()
            {
                QuestionId = invitationEntity.RowKey,
                Message = invitationEntity.EventTopic,
                Recipient = invitationEntity.SlackUserId,
                Botname = "smartbot",
                Image = "https://placebeer.com/300/150",
                Options = new[]
                {
                    new QuestionOption
                    {
                        Text = "Med :+1:",
                        ActionId = RsvpActionIds.Attending,
                        Value = RsvpValues.Attending,
                        Style = ButtonStyles.Primary
                    },
                    new QuestionOption
                    {
                        Text = "Kanskje :man-shrugging:",
                        ActionId = RsvpActionIds.Maybe,
                        Value = RsvpValues.Maybe
                    },
                    new QuestionOption
                    {
                        Text = "Nope :-1:",
                        ActionId = RsvpActionIds.NotAttending,
                        Value = RsvpValues.NotAttending,
                        Style = ButtonStyles.Danger,
                        Confirmation = new QuestionConfirmation
                        {
                            Title = "No va du lite smart",
                            Text = "Sikker?",
                            ConfirmText = "Æ e lite smart",
                            DenyText = "Æ revurdere"
                        }
                    }
                }
            };
            return q;
        }
    }
}