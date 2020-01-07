using System.Net.Http;
using Slackbot.Net.SlackClients.Rtm.BotHelpers;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake;
using Slackbot.Net.SlackClients.Rtm.Connections.Monitoring;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;

namespace Slackbot.Net.SlackClients.Rtm.Connections
{
    internal class ServiceLocator : IServiceLocator
    {
        public SlackConnection CreateConnection(IWebSocketClient websocket, IHandshakeClient handShakeClient, IMentionDetector mentionDetector, IPingPongMonitor pingPongMonitor )
        {
            return new SlackConnection(pingPongMonitor, handShakeClient, mentionDetector, websocket);
        }
    }
}