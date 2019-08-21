using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SlackAPI;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions.Models;
using Slackbot.Net.Core.Utilities;
using Slackbot.Net.Workers;
using SearchSort = SlackAPI.SearchSort;
using SearchSortDirection = SlackAPI.SearchSortDirection;

namespace Slackbot.Net.Core.Integrations.SlackAPIExtensions
{
    /// <summary>
    /// An extension for what is missing in SlackTaskClient
    /// </summary>
    public class SlackTaskClientExtensions : SlackTaskClient
    {
        /// <summary>
        /// Need a seperate bottoken when using the reactions API,
        /// or else the app will post reactions as the user installing the app :/
        /// </summary>
        protected readonly string UserToken;
        protected readonly string AppToken;

        public SlackTaskClientExtensions(IOptions<SlackOptions> slackOptions) : this(slackOptions.Value.Slackbot_SlackApiKey_SlackApp, slackOptions.Value.Slackbot_SlackApiKey_BotUser)
        {

        }

        public SlackTaskClientExtensions(string appToken, string userToken) : base(appToken)
        {
            UserToken = userToken;
            AppToken = appToken;
        }

        /// <summary>
        /// Why:
        /// - The response object in the `SlackAPI` package is lacking returned props (`messages` from Slack
        /// - the `search.messages` API requires a user oauth token.
        ///   See https://api.slack.com/methods/search.messages
        ///
        /// PR can be sent to SlackAPI
        /// </summary>
        public new virtual Task<Models.SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
        {
            var parameters = new List<Tuple<string, string>>();
            parameters.Add(new Tuple<string, string>("query", query));

            if (sorting.HasValue)
                parameters.Add(new Tuple<string, string>("sort", sorting.Value.ToString()));

            if (direction.HasValue)
                parameters.Add(new Tuple<string, string>("sort_dir", direction.Value.ToString()));

            if (enableHighlights)
                parameters.Add(new Tuple<string, string>("highlight", "1"));

            if (count.HasValue)
                parameters.Add(new Tuple<string, string>("count", count.Value.ToString()));

            if (page.HasValue)
                parameters.Add(new Tuple<string, string>("page", page.Value.ToString()));

            return APIRequestWithTokenAsync<SlackAPIExtensions.Models.SearchResponseMessages>(parameters.ToArray());
        }

        /// <summary>
        ///  Why:
        ///  - The SlackAPI method is missing the `thread_ts` parameter (Can be sent as a PR to SlackAPI)
        ///  - The SlackAPI class cannot change the token. For DMs, the bot needs the user token
        /// </summary>
        public Task<PostMessageResponse> PostMessageAsync(
            string channelId,
            string text,
            string botName = null,
            string parse = null,
            bool linkNames = false,
            IBlock[] blocks = null,
            Attachment[] attachments = null,
            bool unfurl_links = false,
            string icon_url = null,
            string icon_emoji = null,
            bool as_user = false,
            string thread_ts = null)
        {
            List<Tuple<string,string>> parameters = new List<Tuple<string,string>>();

            parameters.Add(new Tuple<string,string>("channel", channelId));
            parameters.Add(new Tuple<string,string>("text", text));

            if(!string.IsNullOrEmpty(botName))
                parameters.Add(new Tuple<string,string>("username", botName));

            if (!string.IsNullOrEmpty(parse))
                parameters.Add(new Tuple<string, string>("parse", parse));

            if (linkNames)
                parameters.Add(new Tuple<string, string>("link_names", "1"));

            //addition to SlackAPI
            if (!string.IsNullOrEmpty(thread_ts))
                parameters.Add(new Tuple<string, string>("thread_ts", thread_ts));

            if (blocks != null && blocks.Length > 0)
               parameters.Add(new Tuple<string, string>("blocks", JsonConvert.SerializeObject(blocks,
                  new JsonSerializerSettings()
                  {
                     NullValueHandling = NullValueHandling.Ignore
                  })));

            if (attachments != null && attachments.Length > 0)
                   parameters.Add(new Tuple<string, string>("attachments", JsonConvert.SerializeObject(attachments,
                      new JsonSerializerSettings()
                      {
                         NullValueHandling = NullValueHandling.Ignore
                      })));

            if (unfurl_links)
                parameters.Add(new Tuple<string, string>("unfurl_links", "1"));

            if (!string.IsNullOrEmpty(icon_url))
                parameters.Add(new Tuple<string, string>("icon_url", icon_url));

            if (!string.IsNullOrEmpty(icon_emoji))
                parameters.Add(new Tuple<string, string>("icon_emoji", icon_emoji));

            parameters.Add(new Tuple<string, string>("as_user", as_user.ToString()));

            // Addition to SlackAPI.
            // Necessary for sending a DM as the botuser
            parameters.Add(new Tuple<string, string>("token", UserToken));

            return APIRequestWithTokenAsync<PostMessageResponse>(parameters.ToArray());
        }

        /// <summary>
        /// Missing API in `SlackAPI`
        /// NB! Does not require User token, so could be sent as a PR to SlackAPI
        /// </summary>
        public async Task<string> GetPermalink(string channel, string timestamp)
        {
            var httpClient = new HttpClient();

            // chat.Permalink does not currently support json
            var formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("channel", channel),
                new KeyValuePair<string, string>("message_ts", timestamp)
            });
            var stuff = await formUrlEncodedContent.ReadAsStringAsync();
            var httpContent = new StringContent(stuff, Encoding.UTF8, "application/x-www-form-urlencoded");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://slack.com/api/chat.getPermalink");
            request.Headers.Add("Authorization", $"Bearer {AppToken}");
            request.Content = httpContent;
            var response =  await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var permalink = JsonConvert.DeserializeObject<PermalinkResponse>(content);
            return permalink.Permalink;
        }

        /// <summary>
        /// Why:
        /// - The sender of the reaction should be the bot, not the user of the usertoken
        /// - Need to use the correct token. `SlackAPI` has no way of setting it other than at config time
        /// </summary>
        public new async Task<ReactionAddedResponse> AddReactionAsync(string name, string channel, string ts)
        {
            var httpClient = new HttpClient();
            var stringContent = new Models.Reaction
            {
                Name = name,
                Channel= channel,
                Timestamp = ts
            }.ToSerialized();

            var httpContent = new StringContent(stringContent, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://slack.com/api/reactions.add");
            request.Headers.Add("Authorization", $"Bearer {UserToken}");
            request.Content = httpContent;
            var res = await httpClient.SendAsync(request);
            var content = await res.Content.ReadAsStringAsync();
            var reactionAdded = JsonConvert.DeserializeObject<ReactionAddedResponse>(content);
            return reactionAdded;
        }

        public async Task<IEnumerable<string>> GetMembersOf(string channel)
        {
            var httpClient = new HttpClient();
            var formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("channel", channel),
            });
            var stuff = await formUrlEncodedContent.ReadAsStringAsync();
            var httpContent = new StringContent(stuff, Encoding.UTF8, "application/x-www-form-urlencoded");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://slack.com/api/conversations.members");
            request.Headers.Add("Authorization", $"Bearer {AppToken}");
            request.Content = httpContent;
            var response =  await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var membersResponse = JsonConvert.DeserializeObject<MembersResponse>(content);
            return membersResponse.members;
        }

        /// <summary>
        /// Abstraction over `PostMessageAsync` to make it easier to build interactive DMs
        /// </summary>
        public async Task<PostMessageResponse> PostMessageQuestionAsync(Question question)
        {
            return await PostMessageAsync(question.Recipient, string.Empty, as_user:true, blocks: ToBlocks(question).ToArray());
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