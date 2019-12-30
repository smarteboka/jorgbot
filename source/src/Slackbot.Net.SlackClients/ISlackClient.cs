using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Models.Requests.ChatPostMessage.Minimal;
using Slackbot.Net.SlackClients.Models.Responses;
using Slackbot.Net.SlackClients.Models.Responses.ChatGetPermalink;
using Slackbot.Net.SlackClients.Models.Responses.ChatPostMessage;

namespace Slackbot.Net.SlackClients
{
    public interface ISlackClient
    {
        Task<ChatPostMessageResponse> ChatPostMessage(ChatPostMessageMinimalRequest postMessage);
        Task<ChatGetPermalinkResponse> ChatGetPermalink(string channel, string message_ts);
        Task<Response> ReactionsAdd(string name, string channel, string timestamp);
    }
}