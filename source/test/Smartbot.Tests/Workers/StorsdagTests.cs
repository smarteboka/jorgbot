using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions;
using Slackbot.Net.Workers;
using Smartbot.Utilities;
using Smartbot.Utilities.Storage.Events;
using Smartbot.Utilities.Storsdager.RecurringActions;
using Xunit;

namespace Smartbot.Tests.Workers
{
    public class StorsdagTests
    {
        [Fact(Skip = "Already seeded")]
        public async Task SeedStorsdager()
        {
            var eventStorage = EventsStorage();
            var nesteStorsdager = Timing.GetNextOccurences(StorsdagReminder.LastThursdayOfMonthCron, 12);
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
        public async Task SendsInvitesToAllMembersOfChannel()
        {
            var env = A.Fake<IHostingEnvironment>();
            var logger = A.Fake<ILogger<StorsdagInviter>>();
            var channels = new SlackChannels(env, A.Fake<ILogger<SlackChannels>>());
            var client = SlackClient();
            var eventsStorage = EventsStorage();
            var dontCareCron = "* * * * *";
            var invitationsStorage = InvitationsStorage();
            var inviter = new StorsdagInviter(dontCareCron,logger,eventsStorage, invitationsStorage, client, channels);
            await inviter.Process();
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