using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackAPI;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions;
using Slackbot.Net.Workers;
using SlackConnector.Models;
using Smartbot.Utilities;
using Smartbot.Utilities.Storage;
using Smartbot.Utilities.Storage.Events;
using Smartbot.Utilities.Storsdager.RecurringActions;
using Xunit;
using Xunit.Abstractions;

namespace Smartbot.Tests.Workers
{
    public class StorsdagTests
    {
        private readonly ITestOutputHelper _output;

        public StorsdagTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Skip = "Already seeded")]
        public async Task SeedStorsdager()
        {
            var eventStorage = EventsStorage();
            var nesteStorsdager = Timing.GetNextOccurences(Crons.LastThursdayOfMonthCron, 12);
            var culture = new CultureInfo("nb-NO");

            foreach (var storsdagDate in nesteStorsdager)
            {
                var topic = $"Storsdag  {storsdagDate.Date.ToString(culture.DateTimeFormat.ShortDatePattern, culture)}";
                await eventStorage.Save(new EventEntity(Guid.NewGuid(), EventTypes.StorsdagEventType)
                {
                    Topic = topic,
                    EventTime = storsdagDate
                });
            }

            var knownStorsdag = new DateTime(2019,8,29);
            var fetched = await eventStorage.GetEventsInRange(EventTypes.StorsdagEventType, knownStorsdag, knownStorsdag.AddDays(1));
            Assert.NotEmpty(fetched);
            Assert.Single(fetched);
        }

        [Fact]
        public void GetFutureInviteDates()
        {
            var nesteStorsdager = Timing.GetNextOccurences(Crons.LastThursdayOfMonthCron, 12).ToArray();

            var inviteDates = Timing.GetNextOccurences(Crons.ThirdSaturdayOfMonth, 12).ToArray();

            for (var i = 0; i < 12; i++)
            {
                var stors = nesteStorsdager[i];
                var inviteDay = inviteDates[i];
                _output.WriteLine($"{stors.Day - inviteDay.Day} days before stors. Storsdag {stors}. Invite {inviteDay}");
            }
        }

        [Fact]
        public async Task GetStorsdag()
        {
            var knownStorsdag = new DateTime(2019,8,29);
            var fetched = await EventsStorage().GetEventsInRange(
                EventTypes.StorsdagEventType,
                knownStorsdag,
                knownStorsdag.AddDays(1));
            Assert.NotEmpty(fetched);
            Assert.Single(fetched);
        }

        [Fact]
        public async Task GetNextStorsdag()
        {
            var next = await EventsStorage().GetNextEvent(EventTypes.StorsdagEventType);
            Assert.NotNull(next);
        }

        [Fact(Skip = "no testdata")]
        public async Task DeleteTestData()
        {
            var ok = await EventsStorage().DeleteAll("testtype");
            //var ok = await Storage().DeleteAll(EventTypes.StorsdagEventType);
            Assert.True(ok);
        }

        [Fact]
        public async Task GetInvitations()
        {
            var storage = InvitationsStorage();
            var id = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            await storage.Save(new InvitationsEntity(id)
            {
                EventId = eventId.ToString(),
                EventTime = DateTime.UtcNow,
                EventTopic = "testtopic",
                SlackUserId = "some-user-id",
                Rsvp = RsvpValues.Invited
            });
            var next = await storage.GetInvitations();
            Assert.NotEmpty(next);

            var fetched = await storage.GetById(id.ToString());
            Assert.NotNull(fetched);

            var invitationsByEventId = await storage.GetInvitations(eventId.ToString());
            Assert.NotEmpty(invitationsByEventId);
            Assert.Equal("invited", invitationsByEventId.First().Rsvp);

            var rsvpNew = "yolo";
            await storage.Update(id.ToString(), rsvpNew);

            var afterUpdate = await storage.GetById(id.ToString());
            Assert.Equal("yolo", afterUpdate.Rsvp);
        }

        private static EventsStorage EventsStorage()
        {
            var accountKey = Environment.GetEnvironmentVariable("Smartbot_AzureStorage_AccountKey");
            var storage = new EventsStorage(accountKey);
            return storage;
        }

        private static InvitationsStorage InvitationsStorage()
        {
            var accountKey = Environment.GetEnvironmentVariable("Smartbot_AzureStorage_AccountKey");
            var storage = new InvitationsStorage(accountKey);
            return storage;
        }

        [Fact]
        public async Task SendsInvites()
        {
            var inviter = StorsdagInviter();
            await inviter.Process();
        }

        private StorsdagInviter StorsdagInviter(SlackTaskClientExtensions anotherClient = null)
        {
            var logger = A.Fake<ILogger<StorsdagInviter>>();
            var client = anotherClient ?? SlackClient();
            var eventsStorage = EventsStorage();
            var dontCareCron = "* * * * *";
            var invitationsStorage = InvitationsStorage();
            var inviter = new StorsdagInviter(dontCareCron, logger, eventsStorage, invitationsStorage, client);
            return inviter;
        }

        [Fact]
        public async Task Reminder()
        {
            var slackClient = SlackClient();
            var eventStorage = EventsStorage();
            var nextStorsdag = await eventStorage.GetNextEvent(EventTypes.StorsdagEventType);
            var inviteStorage = InvitationsStorage();
            var invitations = await inviteStorage.GetInvitations(nextStorsdag.RowKey);
//            var unanswered = invitations.Where(i => i.Rsvp == RsvpValues.Invited);
//            Assert.NotEmpty(unanswered);
//            Assert.Equal(3, unanswered.Count());

//            var inviter = StorsdagInviter(slackClient);
//            foreach (var invite in unanswered)
//            {
//                var res = await slackClient.PostMessageAsync(invite.SlackUserId, "Hei, hørte ikke noe fra deg angående storsdag! :/ ", as_user:true);
//                await inviter.SendInviteInSlackDM(invite);
//            }
        }

        public SlackTaskClientExtensions SlackClient()
        {
            var appToken = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_SlackApp");
            var botUserToken = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_BotUser");
            var client = new SlackTaskClientExtensions(appToken, botUserToken);
            return client;
        }
    }
}