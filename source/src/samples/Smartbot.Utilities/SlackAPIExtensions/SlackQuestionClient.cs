using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.SlackClients;
using Slackbot.Net.SlackClients.Models.Requests.ChatPostMessage;
using Slackbot.Net.SlackClients.Models.Requests.ChatPostMessage.Blocks;
using Slackbot.Net.SlackClients.Models.Responses.ChatPostMessage;
using Smartbot.Utilities.SlackAPIExtensions.Models;

namespace Smartbot.Utilities.SlackAPIExtensions
{
 
    public class SlackQuestionClient
    {
        private readonly ISlackClient _slackClient;

        public SlackQuestionClient(ISlackClient slackClient)
        {
            _slackClient = slackClient;
        }

        public async Task<ChatPostMessageResponse> PostMessageQuestionAsync(Question question)
        {
            var chatPost = new ChatPostMessageRequest
            {
                Channel = question.Recipient,
                Text = string.Empty,
                as_user = "true",
                Blocks = ToBlocks(question).ToArray()

            };
            return await _slackClient.ChatPostMessage(chatPost);
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
            yield return new Block
            {
                type = BlockTypes.Image,
                alt_text = "øl",
                image_url = question.Image
            };

            var optionsBlock = new Block()
            {
                type = BlockTypes.Actions,
                block_id = question.QuestionId,
            };
            var elements = new List<Element>();
            foreach (var option in question.Options)
            {
                var element = new Element
                {
                    action_id = option.ActionId,

                    type = ElementTypes.Button,
                    style = option.Style,
                    text = new Text
                    {
                        text = option.Text,
                        type = TextTypes.PlainText
                    },
                    value = option.Value
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
}