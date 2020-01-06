using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound;
using Slackbot.Net.SlackClients.Rtm.Logging;

namespace Slackbot.Net.SlackClients.Rtm.Connections
{
    internal class ConnectionFactory : IConnectionFactory
    {
        public async Task<IWebSocketClient> CreateWebSocketClient(string url)
        {
            var socket = new WebSocketClientLite(new MessageInterpreter(new Logger()));
            await socket.Connect(url);
            return socket;
        }

        public IHandshakeClient CreateHandshakeClient()
        {
            return new FlurlHandshakeClient(new ResponseVerifier());
        }
    }
}