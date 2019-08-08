using System;
using System.Threading.Tasks;
using Smartbot.Utilities;
using Smartbot.Utilities.Storage;
using Xunit;

namespace Oldbot.OldFunction.Tests
{
    public class StorageTests
    {
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