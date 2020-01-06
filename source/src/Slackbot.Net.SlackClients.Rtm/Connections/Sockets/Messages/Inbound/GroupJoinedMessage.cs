using Slackbot.Net.SlackClients.Rtm.Connections.Models;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound
{
    internal class GroupJoinedMessage : InboundMessage
    {
        public GroupJoinedMessage()
        {
            MessageType = MessageType.Group_Joined;
        }

        public Group Channel { get; set; }
    }
}
