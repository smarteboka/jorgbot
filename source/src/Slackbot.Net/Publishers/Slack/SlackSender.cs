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
        private readonly SlackTaskClient _slackAppClient;
        private readonly SlackTaskClient _slackBotUserClient;

        public SlackSender(IOptions<SlackOptions> slackOptions, ILogger<SlackSender> logger) : this(slackOptions.Value.Slackbot_SlackApiKey_SlackApp, slackOptions.Value.Slackbot_SlackApiKey_BotUser)
        {
            _logger = logger;
        }

        public SlackSender(string appToken, string userToken)
        {
            _slackAppClient = new SlackTaskClient(appToken);
            _slackBotUserClient = new SlackTaskClient(userToken);
        }

        public async Task Send(string msg, string channel)
        {
            var res = await _slackAppClient.PostMessageAsync(channel, msg,  linkNames: true);
            if (!res.ok)
            {
                _logger.LogError(res.error);
            }
        }

        public async Task SendQuestion(Question question)
        {
            var res = await _slackBotUserClient.PostMessageAsync(question.Channel,"yo", as_user:true, blocks: ToBlocks(question).ToArray());

            if (!res.ok)
            {
                var response = JsonConvert.SerializeObject(res);
                //_logger.LogError(res.error);
                throw new Exception(response);
            }
        }

        private IEnumerable<IBlock> ToBlocks(Question question)
        {
            yield return new Block
            {
                type = BlockTypes.Section,
                text = new Text
                {
                    text = question.Message,
                    type = TextTypes.PlainText
                }
            };
            var optionsBlock = new Block()
            {
                type = BlockTypes.Actions,

            };
            var elements = new List<Element>();
            foreach (var option in question.Options)
            {
                var element = new Element
                {
                    action_id = option.ActionId,//"storsdag-rsvp-yes",
                    type = ElementTypes.Button,
                    style = ButtonStyles.Primary,
                    text = new Text
                    {
                        text = option.Text, //"Deltar! 🍺",
                        type = TextTypes.PlainText
                    },
                    value = option.Value//"deltar",
                };

                if (option.Confirmation != null)
                {
                    element.confirm = new Confirm
                    {
                        title = new Text() {text = option.Confirmation.Title,type = TextTypes.PlainText},
                        text = new Text() {text = option.Confirmation.Text,type = TextTypes.PlainText},
                        confirm = new Text() {text = option.Confirmation.ConfirmText, type = TextTypes.PlainText},
                        deny = new Text() {text = option.Confirmation.DenyText, type = TextTypes.PlainText}
                    };
                }

                elements.Add(element);
            }

            optionsBlock.elements = elements.ToArray();
            yield return optionsBlock;
        }
    }

    public class Question
    {
        public string Message
        {
            get;
            set;
        }

        public string Channel
        {
            get;
            set;
        }

        public IEnumerable<QuestionOption> Options
        {
            get;
            set;
        }

        public string Botname
        {
            get;
            set;
        }
    }

    public class QuestionOption
    {

        public string Text;

        public string ActionId
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public QuestionConfirmation Confirmation
        {
            get;
            set;
        }
    }

    public class QuestionConfirmation
    {
        public string Title
        {
            get;
            set;
        }

        public string ConfirmText
        {
            get;
            set;
        }

        public string DenyText
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }
    }
}