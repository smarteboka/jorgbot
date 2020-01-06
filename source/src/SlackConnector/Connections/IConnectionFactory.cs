using System.Threading.Tasks;
using SlackConnector.Connections.Clients.Channel;
using SlackConnector.Connections.Clients.File;
using SlackConnector.Connections.Clients.Handshake;
using SlackConnector.Connections.Sockets;

namespace SlackConnector.Connections
{
    internal interface IConnectionFactory
    {
        Task<IWebSocketClient> CreateWebSocketClient(string url, ProxySettings proxySettings);
        IHandshakeClient CreateHandshakeClient();
        IFileClient CreateFileClient();
        IChannelClient CreateChannelClient();
    }
}