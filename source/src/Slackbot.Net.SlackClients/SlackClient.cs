using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.SlackClients.Extensions;
using Slackbot.Net.SlackClients.Models.Requests.ChatPostMessage;
using Slackbot.Net.SlackClients.Models.Responses;
using Slackbot.Net.SlackClients.Models.Responses.ChatGetPermalink;
using Slackbot.Net.SlackClients.Models.Responses.ChatPostMessage;
using Slackbot.Net.SlackClients.Models.Responses.UsersList;

namespace Slackbot.Net.SlackClients
{
    internal class SlackClient : ISlackClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ISlackClient> _logger;

        public SlackClient(HttpClient client, ILogger<ISlackClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// https://api.slack.com/methods/chat.postMessage
        /// Scopes required: `chat:write`
        /// </summary>
        public async Task<ChatPostMessageResponse> ChatPostMessage(string channel, string text)
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("channel", channel),
                new KeyValuePair<string, string>("text", text)
            };
            return await _client.PostParametersAsForm<ChatPostMessageResponse>(parameters, "chat.postMessage", s => _logger.LogTrace(s));
        }

        /// <summary>
        /// https://api.slack.com/methods/chat.postMessage
        /// Scopes required: `chat:write`
        /// </summary>
        public async Task<ChatPostMessageResponse> ChatPostMessage(ChatPostMessageRequest postMessage)
        {
            return await _client.PostJson<ChatPostMessageResponse>(postMessage, "chat.postMessage", s => _logger.LogTrace(s));
        }
        
        /// <summary>
        /// https://api.slack.com/methods/reactions.add
        /// Scopes required: no scopes required
        /// </summary>
        public async Task<ChatGetPermalinkResponse> ChatGetPermalink(string channel, string message_ts)
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("channel", channel),
                new KeyValuePair<string, string>("message_ts", message_ts)
            };
          
            return await _client.PostParametersAsForm<ChatGetPermalinkResponse>(parameters,"chat.getPermalink", s => _logger.LogTrace(s));
        }
        
        /// <summary>
        /// https://api.slack.com/methods/reactions.add
        /// Scopes required: `reactions:write`
        /// </summary>
        public async Task<Response> ReactionsAdd(string name, string channel, string timestamp)
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("name", name),
                new KeyValuePair<string, string>("channel", channel),
                new KeyValuePair<string, string>("timestamp", timestamp)
            };
          
            return await _client.PostParametersAsForm<ChatGetPermalinkResponse>(parameters,"reactions.add", s => _logger.LogTrace(s));
        }
        
        /// <summary>
        /// https://api.slack.com/methods/users.list
        /// Scopes required: `users:read`
        /// </summary>
        public async Task<UsersListResponse> UsersList()
        {
            // var parameters = new List<KeyValuePair<string, string>>
            // {
            //     new KeyValuePair<string, string>("include_locale", "true")
            // };
            return await _client.PostParametersAsForm<UsersListResponse>(null,"users.list", s => _logger.LogTrace(s));
        }
        
    }
}