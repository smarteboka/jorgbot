using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake;
using Slackbot.Net.SlackClients.Rtm.Connections.Monitoring;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;

namespace Slackbot.Net.SlackClients.Rtm.Connections
{
    internal interface IServiceLocator
    {
        IWebSocketClient CreateConnectedWebSocketClient();
        IHandshakeClient CreateHandshakeClient();
        IPingPongMonitor CreatePingPongMonitor();

    }
}