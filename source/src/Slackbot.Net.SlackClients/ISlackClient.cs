using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Models.Requests.ChatPostMessage;
using Slackbot.Net.SlackClients.Models.Responses;
using Slackbot.Net.SlackClients.Models.Responses.ChatGetPermalink;
using Slackbot.Net.SlackClients.Models.Responses.ChatPostMessage;
using Slackbot.Net.SlackClients.Models.Responses.UsersList;

namespace Slackbot.Net.SlackClients
{
    public interface ISlackClient
    {
        Task<ChatPostMessageResponse> ChatPostMessage(string channel, string text);
        Task<ChatGetPermalinkResponse> ChatGetPermalink(string channel, string message_ts);
        Task<Response> ReactionsAdd(string name, string channel, string timestamp);
        Task<ChatPostMessageResponse> ChatPostMessage(ChatPostMessageRequest postMessage);
        Task<UsersListResponse> UsersList();
    }
}