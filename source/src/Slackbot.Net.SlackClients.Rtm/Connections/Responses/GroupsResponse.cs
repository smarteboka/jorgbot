using Slackbot.Net.SlackClients.Rtm.Connections.Models;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Responses
{
    internal class GroupsResponse : StandardResponse
    {
         public Group[] Groups { get; set; }
    }
}