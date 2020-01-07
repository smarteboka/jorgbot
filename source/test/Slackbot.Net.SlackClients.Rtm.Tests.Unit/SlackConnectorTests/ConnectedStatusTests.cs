using System;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Slackbot.Net.SlackClients.Rtm.Connections;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake;
using Slackbot.Net.SlackClients.Rtm.Connections.Models;
using Slackbot.Net.SlackClients.Rtm.Connections.Responses;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;
using Slackbot.Net.SlackClients.Rtm.Exceptions;
using Slackbot.Net.SlackClients.Rtm.Models;
using Xunit;

namespace Slackbot.Net.SlackClients.Rtm.Tests.Unit.SlackConnectorTests
{
    public class ConnectedStatusTests
    {
        private string _slackKey = "slacKing-off-ey?";
        private string _webSocketUrl = "https://some-web-url";
        private readonly Mock<IHandshakeClient> _handshakeClient;
        private readonly Mock<IWebSocketClient> _webSocketClient;
        private readonly Mock<IServiceLocator> _serviceLocator;
        private readonly SlackConnector _slackConnector;

        public ConnectedStatusTests()
        {
            _handshakeClient = new Mock<IHandshakeClient>();
            _webSocketClient = new Mock<IWebSocketClient>();
            _serviceLocator = new Mock<IServiceLocator>();
            _slackConnector = new SlackConnector(_serviceLocator.Object);

            _serviceLocator
                .Setup(x => x.CreateHandshakeClient())
                .Returns(_handshakeClient.Object);

            _serviceLocator
                .Setup(x => x.CreateConnectedWebSocketClient())
                .Returns(_webSocketClient.Object);
        }

        [Fact]
        public async Task should_throw_exception_when_handshake_is_not_ok()
        {
            // given
            var handshakeResponse = new HandshakeResponse
            {
                Ok = false,
                Error = "I AM A ERROR"
            };

            _handshakeClient
                .Setup(x => x.FirmShake(_slackKey))
                .ReturnsAsync(handshakeResponse);

            // when
            var exception = await Assert.ThrowsAsync<HandshakeException>(() => _slackConnector.Connect(_slackKey));

            // then
            exception.Message.ShouldBe(handshakeResponse.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task should_throw_exception_given_empty_api_key(string slackKey)
        {
            // given
            
            // when
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _slackConnector.Connect(slackKey));

            // then
            exception.Message.ShouldContain("slackKey");
        }
    }
}