using System;
using System.Threading.Tasks;
using Smartbot.Storage;
using Xunit;

namespace Oldbot.OldFunction.Tests
{
    public class StorageTests
    {
        [Fact]
        public async Task TestStorage()
        {
            var message = await Save("bølle");
            
            Assert.Equal("bølle", message.User);
            Assert.Equal("yolo, this is a slackmsg", message.Text);
            Assert.Equal("{ \"usr\" : \"@bolle\" }", message.Raw);

            var storedMessages = await Storage().GetAllByUser("bølle");
            Assert.NotEmpty(storedMessages);

            var ok = await Storage().DeleteAllByUser("bølle");
            Assert.True(ok);
        }

        private async Task<SlackMessageEntity> Save(string user)
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