using System.Threading.Tasks;
using SlackConnector.Connections.Clients.Handshake;
using SlackConnector.Connections.Sockets;

namespace SlackConnector.Connections
{
    internal interface IConnectionFactory
    {
        Task<IWebSocketClient> CreateWebSocketClient(string url);
        IHandshakeClient CreateHandshakeClient();
    }
}