using System;
using System.Threading.Tasks;
using Slackbot.Net.Publishers.Slack;
using Xunit;

namespace Slackbot.Net.Tests
{
    public class SlackSenderTests
    {
        [Fact (Skip = "Integration test")]
        public async Task Test1()
        {
            var slackSender = new SlackSender(Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_SlackApp"));
            await slackSender.Send("\\/poll \"where should we do x?\" \"A\" \"B\"", "poller", ":cake:", "#testss");
        }
    }
}