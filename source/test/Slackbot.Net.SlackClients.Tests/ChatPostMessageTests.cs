using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Exceptions;
using Slackbot.Net.SlackClients.Models.Requests.ChatPostMessage.Minimal;
using Xunit;
using Xunit.Abstractions;

namespace Slackbot.Net.Tests
{
    public class ChatPostMessageTests : Setup
    {
        public ChatPostMessageTests(ITestOutputHelper helper) : base(helper)
        {
        }
        
        [Fact]
        public async Task PostMinimalWorks()
        {
            var request = new ChatPostMessageMinimalRequest
            {
                Channel = "testss", 
                Text = "hei"
            };
            var response = await SlackClient.ChatPostMessage(request);
            Assert.True(response.ok);
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