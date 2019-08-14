using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackAPI;
using Slackbot.Net.Core.Integrations.SlackAPI.Extensions;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions.Models;
using Slackbot.Net.Core.Utilities;
using SearchSort = SlackAPI.SearchSort;
using SearchSortDirection = SlackAPI.SearchSortDirection;

namespace Slackbot.Net.Core.Integrations.SlackAPIExtensions
{
    /// <summary>
    /// An extension for what is missing in SlackTaskClient
    /// </summary>
    public class SlackTaskClientExtensions : SlackTaskClient, ISlackClient
    {
        /// <summary>
        /// Need a seperate bottoken when using the reactions API,
        /// or else the app will post reactions as the user installing the app :/
        /// </summary>
        protected readonly string UserToken;
        protected readonly string AppToken;

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
        public new Task<Models.SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
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
        /// Why:
        /// - Unable to set parsing of names correctly,
        ///   @name was not rendered as clickable in some cases
        ///
        /// Needs a re-test to verify.
        /// </summary>
        public async Task<HttpResponseMessage> SendMessage(ChatMessage chatMessage)
        {
            var httpClient = new HttpClient();
            chatMessage.Text = chatMessage.Text;
            var stringContent = chatMessage.ToSerialized();

            var httpContent = new StringContent(stringContent,Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://slack.com/api/chat.postMessage");
            request.Headers.Add("Authorization", $"Bearer {AppToken}");
            request.Content = httpContent;

            return await httpClient.SendAsync(request);
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
    }
}