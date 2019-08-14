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
            var slackSender = new SlackSender(Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_SlackApp"));
            var q = new Question()
            {
                Message = "Det er storsdag denne uka",
                Channel = "#testss",
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