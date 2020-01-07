using System.Linq;
using System.Threading.Tasks;
using Moq;
using Slackbot.Net.SlackClients.Rtm.Connections;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake;
using Slackbot.Net.SlackClients.Rtm.Connections.Models;
using Slackbot.Net.SlackClients.Rtm.Connections.Responses;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;
using Slackbot.Net.SlackClients.Rtm.Models;
using Xunit;

namespace Slackbot.Net.SlackClients.Rtm.Tests.Unit.SlackConnectorTests
{
    public class HubsTests
    {
        private string _webSocketUrl = "some-web-url";
        private readonly Mock<IHandshakeClient> _handshakeClient;
        private readonly SlackConnector _slackConnector;

        public HubsTests()
        {
            _handshakeClient = new Mock<IHandshakeClient>();
            var webSocketClient = new Mock<IWebSocketClient>();
            var connectionFactory = new Mock<IServiceLocator>();
            _slackConnector = new SlackConnector(connectionFactory.Object);

            connectionFactory
                .Setup(x => x.CreateHandshakeClient())
                .Returns(_handshakeClient.Object);

            connectionFactory
                .Setup(x => x.CreateConnectedWebSocketClient())
                .Returns(webSocketClient.Object);
        }

      
    }
}