using System;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions.Models;
using Xunit;

namespace Slackbot.Net.Tests
{
    public class SlackExtensionsTests
    {
        //[Fact (Skip = "Integration test")]
        [Fact]
        public async Task GetUsers()
        {
            var appToken = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_SlackApp");
            var botUserToken = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_BotUser");
            var slackClient = new SlackTaskClientExtensions(appToken, botUserToken);
            var users = await slackClient.GetUserListAsync();
            var allSlackUsers = users.members;
            var filtered = allSlackUsers.Where(u => !u.is_bot && !u.IsSlackBot);
            Assert.Equal(14, filtered.Count());
            var beta = filtered.Where(u => u.name == "johnkors");
            Assert.Single(beta);
        }
    }
}