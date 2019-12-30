using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Models.Requests.ChatPostMessage.Minimal;
using Xunit;
using Xunit.Abstractions;

namespace Slackbot.Net.Tests
{
    public class AddReactionTests : Setup
    {
        public AddReactionTests(ITestOutputHelper helper) : base(helper)
        {
        }
        
        [Fact]
        public async Task AddReactionWorks()
        {
            var request = new ChatPostMessageMinimalRequest
            {
                Channel = "testss", 
                Text = "hei"
            };
            var response = await SlackClient.ChatPostMessage(request);
            var reactionResponse = await SlackClient.ReactionsAdd("thumbsup", response.channel, response.ts);
            Assert.True(reactionResponse.ok);
        }
    }
}