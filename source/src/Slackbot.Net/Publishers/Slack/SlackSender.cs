using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
            string msg = "Det er storsdag denne uka";
            var res = await _client.PostMessageAsync(channel, msg, blocks: Blocks().ToArray());

            if (!res.ok)
            {
                var response = JsonConvert.SerializeObject(res);
                //_logger.LogError(res.error);
                throw new Exception(response);
            }
        }

        private IEnumerable<IBlock> Blocks()
        {
            yield return new Block
            {
                type = BlockTypes.Section,
                text = new Text
                {
                    text = "Det er storsdag!",
                    type = TextTypes.PlainText
                }
            };
            yield return new Block()
            {
                //text = new Text{ text = "Det er storsdag", type = TextTypes.PlainText},
                type = BlockTypes.Actions,
                elements = new[]
                {
                    new Element
                    {
                        type = ElementTypes.Button,
                        style = ButtonStyles.Primary,
                        text = new Text
                        {
                            text = "Deltar! 🍺",
                            type = TextTypes.PlainText
                        },
                        value = "deltar",

                    },
                    new Element
                    {
                        type = ElementTypes.Button,
                        style = ButtonStyles.Danger,
                        text = new Text {text = "Kan ikke 😢️", type = TextTypes.PlainText},
                        value = "deltar-ikke",
                        confirm = new Confirm
                        {
                            title = new Text() {text = "Du er en amøbe",type = TextTypes.PlainText},
                            text = new Text() {text = "Stemmer?",type = TextTypes.PlainText},
                            confirm = new Text() {text = "Innrømmelse", type = TextTypes.PlainText},
                            deny = new Text() {text = "Benektelse", type = TextTypes.PlainText}
                        }
                    },
                }
            };
        }
//
//        private IEnumerable<Attachment> Attachments()
//        {
//            yield return new Attachment()
//            {
//                fallback = "Storsdags invite.",
//                color = "#36a64f",
//                //pretext = "Det er storsdag",
////                author_name = "Bobby Tables",
////                author_link = "http://flickr.com/bobby/",
////                author_icon = "http://flickr.com/icons/bobby.jpg",
//                title = "STORSDAG",
//                //title_link = "https://api.slack.com/",
//                //text = "Mann/mus?",
//                //fields = new[] {new Field() {title = "Priority", value = "High", @short = false}, new Field() {title = "Priority", value = "High", @short = true}, new Field() {title = "Priority", value = "High", @short = true}},
//                actions = new[]
//                {
//                    new AttachmentAction("storsdag-ja", "Jeg er med!")
//                    {
//                        style = "primary",
//                        value = "storsdag-ja"
//
//                    },
//                    new AttachmentAction("storsdag-nei", "Sry, kan ikke") {
//                        style = "danger",
//                        value = "storsdag-nei",
//                        confirm = new ActionConfirm
//                        {
//                            title = "Jeg er en amøbe",
//                            text = "Stemmer?",
//                            dismiss_text = "Æ angre mæ",
//                            ok_text = "Innrømmelse"
//                        }},
//                }
//            };
//        }
    }
}