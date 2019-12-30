using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Models.Responses.SearchMessages;

namespace Slackbot.Net.SlackClients
{
    public interface ISearchClient
    {
        Task<SearchMessagesResponse> SearchMessagesAsync(string query);
    }
}