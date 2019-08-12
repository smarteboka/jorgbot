using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackAPI;

namespace Slackbot.Net.Publishers.Slack
{
    public class SlackSender
    {
        private readonly ILogger<SlackSender> _logger;
        private readonly SlackTaskClient _client;

        public SlackSender(IOptions<SlackOptions> slackOptions, ILogger<SlackSender> logger) : this(slackOptions.Value.Slackbot_SlackApiKey_SlackApp)
        {
            _logger = logger;
        }

        public SlackSender(string token)
        {
            _client = new SlackTaskClient(token);
        }

        public async Task Send(string msg, string botName, string iconEmoji, string channel)
        {
            var res = await _client.PostMessageAsync(channel, msg, botName: botName, linkNames: true, icon_emoji: iconEmoji);
            if (!res.ok)
            {
                _logger.LogError(res.error);
            }
        }

        public async Task SendQuestion()
        {
            string channel = "#testss";
            string msg = "yo";
            await _client.PostMessageAsync(channel, msg, blocks: new[]
            {
                new Block
                {
                    type = "section",
                    text = new Text{ text = "Stordag - er du med?"}
                },
                new Block
                {
                    type = "actions",
                    elements = new[]
                    {
                        new Element
                        {
                            type = "button",
                            text = new Text
                            {
                                type = "plain_text",
                                text = ":+1:"
                            }
                        },
                        new Element
                        {
                            type = "button",
                            text = new Text
                            {
                                type = "plain_text",
                                text = ":-1:",
                                emoji = true
                            }
                        }
                    }
                },
            });
        }
    }
}