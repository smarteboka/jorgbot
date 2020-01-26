using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.SlackClients;
using Slackbot.Net.SlackClients.Http;
using Slackbot.Net.SlackClients.Http.Models.Requests.ChatPostMessage.Blocks;
using Slackbot.Net.SlackClients.Http.Models.Responses.UsersList;
using Smartbot.Data.Storage.Events;
using Smartbot.Utilities.SlackAPIExtensions;
using Smartbot.Utilities.SlackAPIExtensions.Models;
using Smartbot.Utilities.SlackQuestions;

namespace Smartbot.Utilities.Storsdager.RecurringActions
{
    public class StorsdagInviter
    {
        private readonly IEventsStorage _eventStorage;
        private readonly IInvitationsStorage _inviteStorage;
        private readonly ISlackClient _slackClient;
        private readonly SlackQuestionClient _questioner;
        private readonly ILogger<StorsdagInviter> _logger;

        public StorsdagInviter(IEventsStorage eventStorage, IInvitationsStorage inviteStorage, ISlackClient slackClient, SlackQuestionClient questioner, ILogger<StorsdagInviter> logger)
        {
            _eventStorage = eventStorage;
            _inviteStorage = inviteStorage;
            _slackClient = slackClient;
            _questioner = questioner;
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
                    var members = await _slackClient.UsersList();
                    var allSlackUsers = members.Members;
                    var filtered = allSlackUsers.Where(u => !u.Is_Bot);
                    //filtered = filtered.Where(u => u.Name == "johnkors");
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
            //unanswered = unanswered.Where(u => u.SlackUsername == "johnkors");

            foreach (var invite in unanswered)
            {
                _logger.LogInformation($"Reminding {invite.SlackUsername} about {invite.EventTopic}");
                var res = await _slackClient.ChatPostMessage(invite.SlackUserId, "Hei, hørte ikke noe fra deg angående storsdag! :/ ");
                await SendInviteInSlackDM(invite);
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
                SlackUsername = user.Name,
                SlackUserId = user.Id,
                Rsvp = RsvpValues.Invited
            };
            await _inviteStorage.Save(invite);
            var inviteSentOk = await SendInviteInSlackDM(invite);

            if(inviteSentOk)
            {
                _logger.LogInformation($"Sent invite {nextStorsdag.Topic} to {user.Name}");
            }
            else
            {
                _logger.LogError($"Could not send invite {nextStorsdag.Topic} to {user.Name}");
            }
        }

        public async Task<bool> SendInviteInSlackDM(InvitationsEntity invite)
        {
            var q = AttendStorsdagQuestion(invite);
            var res = await _questioner.PostMessageQuestionAsync(q);

            if(!res.Ok)
                throw new Exception(res.Error);

            return res.Ok;
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