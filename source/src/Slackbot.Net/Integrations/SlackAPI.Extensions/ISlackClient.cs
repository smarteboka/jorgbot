using System.Net.Http;
using System.Threading.Tasks;
using Slackbot.Net.Utilities.SlackAPIFork;

namespace Slackbot.Net.Utilities.SlackAPI.Extensions
{
    public interface ISlackClient
    {
        Task<SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null);
        Task<HttpResponseMessage> SendMessage(ChatMessage chatMessage);
        Task<HttpResponseMessage[]> AddReactions(string channelId, string thread_ts);
        Task<string> GetPermalink(string channel, string timestamp);
    }
}