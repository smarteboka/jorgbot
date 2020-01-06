using Slackbot.Net.SlackClients.Rtm.Connections.Models;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound
{
    internal class UserJoinedMessage : InboundMessage
    {
        public UserJoinedMessage()
        {
            MessageType = MessageType.Team_Join;
        }

        public User User { get; set; }
    }
}
