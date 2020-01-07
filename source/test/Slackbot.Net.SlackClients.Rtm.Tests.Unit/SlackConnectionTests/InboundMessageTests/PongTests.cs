using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using Shouldly;
using Slackbot.Net.SlackClients.Rtm.BotHelpers;
using Slackbot.Net.SlackClients.Rtm.Connections;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake;
using Slackbot.Net.SlackClients.Rtm.Connections.Monitoring;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound;
using Slackbot.Net.SlackClients.Rtm.Models;
using Xunit;

namespace Slackbot.Net.SlackClients.Rtm.Tests.Unit.SlackConnectionTests.InboundMessageTests
{
    public class PongTests
    {
        [Theory, AutoMoqData]
        private async Task should_raise_event(
            Mock<IWebSocketClient> webSocket, 
            Mock<IHandshakeClient> handShakeClient,
            Mock<IMentionDetector> mentionDetector,
            Mock<IPingPongMonitor> pingPongMonitor,
            ServiceLocator serviceLocator)
        {
            // given
            var slackConnection = serviceLocator.CreateConnection(webSocket.Object, handShakeClient.Object, mentionDetector.Object, pingPongMonitor.Object);

            var connectionInfo = new ConnectionInformation();
            await slackConnection.Initialise(connectionInfo);

            DateTime lastTimestamp = DateTime.MinValue;
            
            slackConnection.OnPong += timestamp =>
            {
                lastTimestamp = timestamp;
                return Task.CompletedTask;
            };
            
            var inboundMessage = new PongMessage
            {
                Timestamp = DateTime.Now
            };

            // when
            webSocket.Raise(x => x.OnMessage += null, null, inboundMessage);

            // then
            lastTimestamp.ShouldBe(inboundMessage.Timestamp);
        }

        [Theory, AutoMoqData]
        private async Task should_pong_monitor( 
            Mock<IPingPongMonitor> pingPongMonitor,
            Mock<IWebSocketClient> webSocket, 
            Mock<IHandshakeClient> handShakeClient,
            ServiceLocator serviceLocator)
        {
            // given
            var slackConnection = serviceLocator.CreateConnection(webSocket.Object, handShakeClient.Object, null, pingPongMonitor.Object);
            var connectionInfo = new ConnectionInformation();
            await slackConnection.Initialise(connectionInfo);

            var inboundMessage = new PongMessage
            {
                Timestamp = DateTime.Now
            };

            // when
            webSocket.Raise(x => x.OnMessage += null, null, inboundMessage);

            // then
            pingPongMonitor.Verify(x => x.Pong(), Times.Once);
        }
    }
}