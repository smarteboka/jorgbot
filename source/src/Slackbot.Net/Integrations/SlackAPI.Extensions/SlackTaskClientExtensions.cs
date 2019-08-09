using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Slackbot.Net.Utilities;

namespace Slackbot.Net.Integrations.SlackAPI.Extensions
{
    public class SlackTaskClientExtensions : SlackAPIFork.SlackTaskClient, ISlackClient
    {
        /// <summary>
        /// Need a seperate bottoken when using the reactions API,
        /// or else the app will post reactions as the user installing the app :/
        /// </summary>
        protected readonly string BotToken;

        public SlackTaskClientExtensions(string appToken, string bottoken) : base(appToken)
        {
            BotToken = bottoken;
        }

        public async Task<HttpResponseMessage> SendMessage(ChatMessage chatMessage)
        {
            var httpClient = new HttpClient();
            var stringContent = chatMessage.ToSerialized();

            var httpContent = new StringContent(stringContent,Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://slack.com/api/chat.postMessage");
            request.Headers.Add("Authorization", $"Bearer {SlackToken}");
            request.Content = httpContent;

            return await httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage[]> AddReactions(string channelId, string thread_ts)
        {
            var t1 = React(channelId, thread_ts, "older_man");
            var t2 = React(channelId, thread_ts, "older_man::skin-tone-2");
            var t3 = React(channelId, thread_ts, "older_man::skin-tone-3");
            var t4 = React(channelId, thread_ts, "older_man::skin-tone-4");
            var t5 = React(channelId, thread_ts, "older_man::skin-tone-5");
            var res = await Task.WhenAll(t1, t2, t3, t4, t5);
            return res;
        }

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
            request.Headers.Add("Authorization", $"Bearer {BotToken}");
            request.Content = httpContent;
            var response =  await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var permalink = JsonConvert.DeserializeObject<PermalinkResponse>(content);
            return permalink.Permalink;
        }

        private async Task<HttpResponseMessage> React(string channelId, string thread_ts, string olderMan)
        {
            var httpClient = new HttpClient();
            var stringContent = new Reaction
            {
                Name = olderMan,
                Channel = channelId,
                Timestamp = thread_ts
            }.ToSerialized();

            var httpContent = new StringContent(stringContent, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://slack.com/api/reactions.add");
            request.Headers.Add("Authorization", $"Bearer {BotToken}");
            request.Content = httpContent;
            return await httpClient.SendAsync(request);
        }
    }
}