using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Extensions;
using Slackbot.Net.SlackClients.Models.Responses.SearchMessages;

namespace Slackbot.Net.SlackClients
{
    /// <inheritdoc/>

    internal class SearchClient : ISearchClient
    {
        private readonly HttpClient _httpClient;

        public SearchClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc/>

        public async Task<SearchMessagesResponse> SearchMessagesAsync(string query)
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("query", query)
            };
            return await _httpClient.PostParametersAsForm<SearchMessagesResponse>(parameters, "search.messages");
        }
    }
}