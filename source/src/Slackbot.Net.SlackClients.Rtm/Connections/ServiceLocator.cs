using System.Net.Http;
using Slackbot.Net.SlackClients.Rtm.BotHelpers;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake;
using Slackbot.Net.SlackClients.Rtm.Connections.Monitoring;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound;
using Slackbot.Net.SlackClients.Rtm.Logging;

namespace Slackbot.Net.SlackClients.Rtm.Connections
{
    internal class ServiceLocator : IServiceLocator
    {
        public IWebSocketClient CreateConnectedWebSocketClient()
        {
            return new WebSocketClientLite(new MessageInterpreter(new Logger()));
        }

        public IHandshakeClient CreateHandshakeClient()
        {
            return new HandshakeClient(new HttpClient());
        }

        public IPingPongMonitor CreatePingPongMonitor()
        {
            return new PingPongMonitor(new Timer(), new DateTimeKeeper());
        }

        public SlackConnection CreateConnection(IWebSocketClient websocket, IMentionDetector mentionDetector = null, IPingPongMonitor pingPongMonitor = null)
        {
            return new SlackConnection(pingPongMonitor ?? CreatePingPongMonitor(), CreateHandshakeClient(), mentionDetector ?? new MentionDetector(), websocket);
        }
    }
}