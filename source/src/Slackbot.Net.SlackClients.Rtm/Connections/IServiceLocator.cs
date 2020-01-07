using Slackbot.Net.SlackClients.Rtm.BotHelpers;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake;
using Slackbot.Net.SlackClients.Rtm.Connections.Monitoring;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;

namespace Slackbot.Net.SlackClients.Rtm.Connections
{
    internal interface IServiceLocator
    {
        SlackConnection CreateConnection(IWebSocketClient websocket, IHandshakeClient handshakeClient, IMentionDetector mentionDetector = null, IPingPongMonitor pingPongMonitor = null);

    }
}