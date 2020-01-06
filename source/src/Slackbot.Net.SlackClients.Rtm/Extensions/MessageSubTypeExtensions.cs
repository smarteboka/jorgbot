using Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound;
using Slackbot.Net.SlackClients.Rtm.Models;

namespace Slackbot.Net.SlackClients.Rtm.Extensions
{
    internal static class MessageSubTypeExtensions
    {
        public static SlackMessageSubType ToSlackMessageSubType(this MessageSubType subType)
        {
            return (SlackMessageSubType)subType;
        }
    }
}