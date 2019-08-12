using System;
using System.Threading.Tasks;
using Slackbot.Net.Publishers.Slack;
using Xunit;

namespace Slackbot.Net.Tests
{
    public class SlackSenderTests
    {
        //[Fact (Skip = "Integration test")]
        [Fact]
        public async Task SendsAMessageToSlack()
        {
            var slackSender = new SlackSender(Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_SlackApp"));
            await slackSender.SendQuestion();
        }
    }
}