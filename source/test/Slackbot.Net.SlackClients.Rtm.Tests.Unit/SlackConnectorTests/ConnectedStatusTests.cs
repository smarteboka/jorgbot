using System;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake;
using Slackbot.Net.SlackClients.Rtm.Connections.Monitoring;
using Slackbot.Net.SlackClients.Rtm.Connections.Responses;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;
using Slackbot.Net.SlackClients.Rtm.Exceptions;
using Xunit;

namespace Slackbot.Net.SlackClients.Rtm.Tests.Unit.SlackConnectorTests
{
    public class ConnectedStatusTests
    {
        private string _slackKey = "slacKing-off-ey?";
        private readonly Mock<IHandshakeClient> _handshakeClient;
        private readonly SlackConnector _slackConnector;

        public ConnectedStatusTests()
        {
            _handshakeClient = new Mock<IHandshakeClient>();
            var webSocketClient = new Mock<IWebSocketClient>();
            var pingPong = new Mock<IPingPongMonitor>();
            _slackConnector = new SlackConnector(_handshakeClient.Object, webSocketClient.Object, pingPong.Object);
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