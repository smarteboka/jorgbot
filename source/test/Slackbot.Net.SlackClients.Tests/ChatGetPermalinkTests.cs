using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Exceptions;
using Slackbot.Net.SlackClients.Models.Requests.ChatPostMessage.Minimal;
using Xunit;
using Xunit.Abstractions;

namespace Slackbot.Net.Tests
{
    public class ChatGetPermalinkTests : Setup
    {
        public ChatGetPermalinkTests(ITestOutputHelper helper) : base(helper)
        {
        }
        
        [Fact]
        public async Task GetPermalinkWorks()
        {
            var request = new ChatPostMessageMinimalRequest
            {
                Channel = "testss", 
                Text = "hei"
            };
            var response = await SlackClient.ChatPostMessage(request);
            var permalink = await SlackClient.ChatGetPermalink(response.channel, response.ts);
            Assert.True(permalink.ok);
            Assert.NotNull(permalink.Permalink);
        }
        
        [Fact]
        public async Task PostMissingChannelThrowsSlackApiException()
        {
            var request = new ChatPostMessageMinimalRequest
            {
                Channel = "thisdoesnotexist", 
                Text = "hei"
            };
            await Assert.ThrowsAsync<SlackApiException>(() => SlackClient.ChatPostMessage(request));
        }
    }
}