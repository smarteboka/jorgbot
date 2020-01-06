using Slackbot.Net.SlackClients.Rtm.Connections.Models;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Responses
{
    internal class ChannelsResponse : StandardResponse
    {
         public Channel[] Channels { get; set; }
    }
}