using System.Net.Http;
using System.Threading.Tasks;
using Slackbot.Net.Integrations.SlackAPIFork;

namespace Slackbot.Net.Integrations.SlackAPI.Extensions
{
    public interface ISlackClient
    {
        Task<SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null);
        Task<HttpResponseMessage> SendMessage(ChatMessage chatMessage);
        Task<HttpResponseMessage[]> AddReactions(string channelId, string thread_ts);
        Task<string> GetPermalink(string channel, string timestamp);
    }
}