using System;
using System.Threading.Tasks;
using FakeItEasy;
using Newtonsoft.Json;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions.Models;
using Slackbot.Net.Workers.Publishers.Slack;
using Xunit;

namespace Slackbot.Net.Tests
{
    public class SlackExtensionsTests
    {
        //[Fact (Skip = "Integration test")]
        [Fact]
        public async Task PostsQuestionAsDM()
        {
            var appToken = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_SlackApp");
            var botUserToken = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_BotUser");
            var slackTaskClientExtensions = new SlackTaskClientExtensions(appToken, botUserToken);
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
            var res = await slackTaskClientExtensions.PostMessageQuestionAsync(q);
            Assert.True(res.ok);
        }
    }
}