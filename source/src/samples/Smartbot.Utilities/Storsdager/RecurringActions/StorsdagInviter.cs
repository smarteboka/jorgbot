using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackAPI;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions.Models;
using Slackbot.Net.Workers;
using Slackbot.Net.Workers.Configuration;
using Smartbot.Utilities.Storage.Events;

namespace Smartbot.Utilities.Storsdager.RecurringActions
{
    public class StorsdagInviter : RecurringAction
    {
        private readonly EventsStorage _eventStorage;
        private readonly InvitationsStorage _inviteStorage;
        private readonly SlackTaskClientExtensions _slackClient;
        private readonly SlackChannels _channels;

        public StorsdagInviter(IOptionsSnapshot<CronOptions> options,
            ILogger<RecurringAction> logger,
            EventsStorage eventStorage,
            InvitationsStorage inviteStorage,
            SlackTaskClientExtensions slackClient,
            SlackChannels channels) : base(options, logger)
        {
            _eventStorage = eventStorage;
            _inviteStorage = inviteStorage;
            _slackClient = slackClient;
            _channels = channels;
        }

        public StorsdagInviter(string cron,
            ILogger<RecurringAction> logger,
            EventsStorage eventStorage,
            InvitationsStorage inviteStorage,
            SlackTaskClientExtensions slackClient,
            SlackChannels channels) : base(cron, logger)
        {
            _eventStorage = eventStorage;
            _inviteStorage = inviteStorage;
            _slackClient = slackClient;
            _channels = channels;
        }

        public override async Task Process()
        {
            var nextStorsdag = await _eventStorage.GetNextEvent(EventTypes.StorsdagEventType);
            if (nextStorsdag != null)
            {
                var invitations = await _inviteStorage.GetInvitations(nextStorsdag.RowKey);
                if (!invitations.Any())
                {
                    var members = await _slackClient.GetMembersOf(_channels.TestChannel);
                    foreach (var member in members)
                    {
                        await Invite(member, nextStorsdag);
                    }
                }
            }
            else
            {
                Logger.LogWarning($"No events found.");
            }
        }

        private async Task Invite(string member, EventEntity nextStorsdag)
        {
            var invite = new InvitationsEntity(Guid.NewGuid())
            {
                EventId = nextStorsdag.RowKey,
                EventTime = nextStorsdag.EventTime,
                EventTopic = nextStorsdag.Topic,
                SlackUserId = member,
                Rsvp = RsvpValues.Invited
            };
            await _inviteStorage.Save(invite);
            var inviteSentOk = await SendInviteInSlackDM(invite);

            if(inviteSentOk)
            {
                Logger.LogInformation($"Sent invite {nextStorsdag.Topic} to {member}");
            }
            else
            {
                Logger.LogError($"Could not send invite {nextStorsdag.Topic} to {member}");
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