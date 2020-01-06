using Slackbot.Net.SlackClients.Rtm.Connections.Models;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Responses
{
    internal class UsersResponse : StandardResponse
    {
         public User[] Members { get; set; }
    }
}