using System;
using System.Threading.Tasks;
using FakeItEasy;
using Newtonsoft.Json;
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
            var appToken = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_SlackApp");
            var botUserToken = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_BotUser");
            var slackSender = new SlackSender(appToken,botUserToken);
            var q = new Question()
            {
                Message = "Dette er uka si",
                Channel = "@johnkors",
                Botname = "smartbot",
                Options = new []
                {
                    new QuestionOption { Text = "yes", ActionId = "storsdag-rsvp-yes", Value= "deltar"},
                    new QuestionOption { Text = "no", ActionId = "storsdag-rsvp-no", Value = "deltar ikke", Confirmation = new QuestionConfirmation
                    {
                        Title = "rly title?",
                        Text = "u have to b sure",
                        ConfirmText = "sure",
                        DenyText = "nop"
                    }}
                }
            };
            await slackSender.SendQuestion(q);
        }
    }
}