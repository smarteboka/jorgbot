using System.Net.Http;
using System.Threading.Tasks;
using SlackAPI;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions.Models;
using SearchResponseMessages = Slackbot.Net.Core.Integrations.SlackAPIExtensions.Models.SearchResponseMessages;
using SearchSort = SlackAPI.SearchSort;
using SearchSortDirection = SlackAPI.SearchSortDirection;

namespace Slackbot.Net.Core.Integrations.SlackAPI.Extensions
{
    public interface ISlackClient
    {
        Task<SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null);
        Task<HttpResponseMessage> SendMessage(ChatMessage chatMessage);
        Task<string> GetPermalink(string channel, string timestamp);
        Task<ReactionAddedResponse> AddReactionAsync(string name, string channelId, string threadTs);
    }
}