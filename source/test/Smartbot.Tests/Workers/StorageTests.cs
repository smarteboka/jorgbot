using System;
using System.Linq;
using System.Threading.Tasks;
using Smartbot.Data.Storage.SlackUrls;
using Xunit;
using Xunit.Abstractions;

namespace Smartbot.Tests.Workers
{
    public class StorageTests
    {
        private readonly ITestOutputHelper _helper;

        public StorageTests(ITestOutputHelper helper)
        {
            _helper = helper;
        }
        
        [Fact]
        public async Task TestUrlStorage()
        {
            var message = await Save("bølle");

            Assert.Equal("bølle", message.User);
            Assert.Equal("yolo, this is a slackmsg", message.Text);
            Assert.Equal("{ \"usr\" : \"@bolle\" }", message.Raw);

            var storedMessages = await Storage().GetAllUrlsByUser("bølle");
            Assert.NotEmpty(storedMessages);

            var ok = await Storage().DeleteAllUrlsByUser("bølle");
            Assert.True(ok);
        }

        [Fact]
        public async Task TestMessageStorage()
        {
            var message = await SaveMessage("bølle");

            Assert.Equal("bølle", message.User);
            Assert.Equal("yolo, this is a slackmsg", message.Text);
            Assert.Equal("{ \"usr\" : \"@bolle\" }", message.Raw);

            var storedMessages = await Storage().GetAllMessagesByUser("bølle");
            Assert.NotEmpty(storedMessages);

            var ok = await Storage().DeleteAllMessagesByUser("bølle");
            Assert.True(ok);
        }

        [Fact(Skip = "Run on demand")]
        public async Task DeleteDuplicates()
        {
            var users = await Storage().GetUniqueUsersForUrls();
            foreach (var user in users)
            {
                _helper.WriteLine($"****{user}*****");
                var allMessagesForUser = await Storage().GetAllUrlsByUser("thodd");
                var duplicatGroups = allMessagesForUser.GroupBy(g => g.SlackTimestamp).OrderByDescending(g => g.Key);
                var duplicatesFound = false;
                foreach (var duplicateGroup in duplicatGroups)
                {
                    if (duplicateGroup.Count() > 1)
                    {
                        _helper.WriteLine($"{duplicateGroup.Last().Url}\n{duplicateGroup.First().Url}\n\n");
                        // var ok = await Storage().DeleteMessage(duplicateGroup.Last());
                        // _helper.WriteLine($"{ok}");
                        duplicatesFound = true;
                    }
                }

                if (!duplicatesFound)
                {
                    _helper.WriteLine($"{user}: No duplicates!");
                }
                _helper.WriteLine($"****{user}-end*****");

            }
            
         
        }

        private async Task<SlackUrlEntity> Save(string user)
        {
            var slackMessageEntity = new SlackUrlEntity(Guid.NewGuid())
            {
                User = user,
                Raw = "{ \"usr\" : \"@bolle\" }",
                Text = "yolo, this is a slackmsg"
            };
            return await Storage().Save(slackMessageEntity);
        }

        private async Task<SlackMessageEntity> SaveMessage(string user)
        {
            var slackMessageEntity = new SlackMessageEntity(Guid.NewGuid())
            {
                User = user,
                Raw = "{ \"usr\" : \"@bolle\" }",
                Text = "yolo, this is a slackmsg"
            };
            return await Storage().Save(slackMessageEntity);
        }

        private static SlackMessagesStorage Storage()
        {
            var accountKey = Environment.GetEnvironmentVariable("Smartbot_AzureStorage_AccountKey");
            var storage = new SlackMessagesStorage(accountKey);
            return storage;
        }
    }
}