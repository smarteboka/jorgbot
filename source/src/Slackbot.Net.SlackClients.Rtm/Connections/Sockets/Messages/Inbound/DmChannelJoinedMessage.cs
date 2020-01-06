using Slackbot.Net.SlackClients.Rtm.Connections.Models;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound
{
    internal class DmChannelJoinedMessage : InboundMessage
    {
        public DmChannelJoinedMessage()
        {
            MessageType = MessageType.Im_Created;
        }

        public Im Channel { get; set; }
    }
}
