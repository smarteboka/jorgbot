using System.Net.Http;
using System.Threading.Tasks;
using Slackbot.Net.Integrations.SlackAPI.Extensions;
using Slackbot.Net.Integrations.SlackAPIFork;

namespace Smartbot.Tests
{
    public class MockClient : ISlackClient
    {
        public MockClient()
        {

        }

        public Task<SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
        {
            return Task.FromResult(SearchResponse);
        }

        public SearchResponseMessages SearchResponse { get; set; }

        public Task<HttpResponseMessage> SendMessage(ChatMessage chatMessage)
        {
            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent("{}")
            };
            return Task.FromResult(httpResponseMessage);
        }

        public Task<HttpResponseMessage[]> AddReactions(string channelId, string thread_ts)
        {
            var httpResponseMessage = new []
            {
                new HttpResponseMessage
                {
                    Content = new StringContent("{}"),

                }
            };
            return Task.FromResult(httpResponseMessage);
        }

        public Task<string> GetPermalink(string channel, string timestamp)
        {
            return Task.FromResult(string.Empty);
        }

        public void SetSearchResponse(ContextMessage contextMessage)
        {
            SearchResponse = new SearchResponseMessages
            {
                messages = new SearchResponseMessagesContainer
                {
                    matches = new[]
                    {
                        contextMessage
                    }
                }
            };
        }
    }
}