using System.Net.Http;
using System.Threading.Tasks;
using Slackbot.Net.Utilities.SlackAPIFork;

namespace Slackbot.Net.Utilities.SlackAPI.Extensions
{
    public class NoopClient : ISlackClient
    {
        public Task<SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
        {
            return Task.FromResult(new SearchResponseMessages()
            {
                messages = new SlackAPIFork.SearchResponseMessagesContainer
                {
                    matches = new SlackAPIFork.ContextMessage[0]
                }
            });
        }
        
        public Task<HttpResponseMessage> SendMessage(string channel, string message, string eventTs, string permalink)
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
                    Content = new StringContent("{}")
                }
            };
            return Task.FromResult(httpResponseMessage);
        }

        public Task<string> GetPermalink(string channel, string timestamp)
        {
            return Task.FromResult(string.Empty);
        }
    }
}