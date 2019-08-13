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

        [Fact]
        public async Task HandlesInteractivePayloads()
        {

            var responsePost = "payload=%7B%22type%22%3A%22block_actions%22%2C%22team%22%3A%7B%22id%22%3A%22T0EC3DG3A%22%2C%22domain%22%3A%22smarteboka%22%7D%2C%22user%22%3A%7B%22id%22%3A%22U0EBWMGG4%22%2C%22username%22%3A%22johnkors%22%2C%22name%22%3A%22johnkors%22%2C%22team_id%22%3A%22T0EC3DG3A%22%7D%2C%22api_app_id%22%3A%22AGXHANA5D%22%2C%22token%22%3A%222COph0MFkJiRLJOv8VshaeuK%22%2C%22container%22%3A%7B%22type%22%3A%22message%22%2C%22message_ts%22%3A%221565674899.002500%22%2C%22channel_id%22%3A%22CH1G2JHB8%22%2C%22is_ephemeral%22%3Afalse%7D%2C%22trigger_id%22%3A%22713064355091.14411458112.7c21a87b9be4bdbb6dc6f8c617cf7e41%22%2C%22channel%22%3A%7B%22id%22%3A%22CH1G2JHB8%22%2C%22name%22%3A%22testss%22%7D%2C%22message%22%3A%7B%22type%22%3A%22message%22%2C%22subtype%22%3A%22bot_message%22%2C%22text%22%3A%22Det+er+storsdag+denne+uka%22%2C%22ts%22%3A%221565674899.002500%22%2C%22username%22%3A%22smartbot%22%2C%22bot_id%22%3A%22BGXHSRDFH%22%2C%22blocks%22%3A%5B%7B%22type%22%3A%22section%22%2C%22block_id%22%3A%22kx%3DI2%22%2C%22text%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Det+er+storsdag%21%22%2C%22emoji%22%3Atrue%7D%7D%2C%7B%22type%22%3A%22actions%22%2C%22block_id%22%3A%2220Dbw%22%2C%22elements%22%3A%5B%7B%22type%22%3A%22button%22%2C%22action_id%22%3A%22rGbI%22%2C%22text%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Deltar%21+%3Abeer%3A%22%2C%22emoji%22%3Atrue%7D%2C%22style%22%3A%22primary%22%2C%22value%22%3A%22deltar%22%7D%2C%7B%22type%22%3A%22button%22%2C%22action_id%22%3A%22GS1A4%22%2C%22text%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Kan+ikke+%3Acry%3A%22%2C%22emoji%22%3Atrue%7D%2C%22style%22%3A%22danger%22%2C%22value%22%3A%22deltar-ikke%22%2C%22confirm%22%3A%7B%22title%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Du+er+en+am%5Cu00f8be%22%2C%22emoji%22%3Atrue%7D%2C%22text%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Stemmer%3F%22%2C%22emoji%22%3Atrue%7D%2C%22confirm%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Innr%5Cu00f8mmelse%22%2C%22emoji%22%3Atrue%7D%2C%22deny%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Benektelse%22%2C%22emoji%22%3Atrue%7D%7D%7D%5D%7D%5D%7D%2C%22response_url%22%3A%22https%3A%5C%2F%5C%2Fhooks.slack.com%5C%2Factions%5C%2FT0EC3DG3A%5C%2F724040023300%5C%2F4lvgE22yz95N0zhKVOUTftPu%22%2C%22actions%22%3A%5B%7B%22action_id%22%3A%22rGbI%22%2C%22block_id%22%3A%2220Dbw%22%2C%22text%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Deltar%21+%3Abeer%3A%22%2C%22emoji%22%3Atrue%7D%2C%22value%22%3A%22deltar%22%2C%22style%22%3A%22primary%22%2C%22type%22%3A%22button%22%2C%22action_ts%22%3A%221565674913.292884%22%7D%5D%7D";
            var payloadhandler = new InteractiveResponseHandler(new DummyLogger());
            var res = await payloadhandler.RespondToSlackInteractivePayload(responsePost);
            Assert.Equal("OK", res.Text);
        }
    }
}