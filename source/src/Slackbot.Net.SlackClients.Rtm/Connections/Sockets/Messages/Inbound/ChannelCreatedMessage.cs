using Slackbot.Net.SlackClients.Rtm.Connections.Models;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound
{
    internal class ChannelCreatedMessage : InboundMessage
    {
        public ChannelCreatedMessage()
        {
            MessageType = MessageType.Channel_Created;
        }

        public Channel Channel { get; set; }
    }
}