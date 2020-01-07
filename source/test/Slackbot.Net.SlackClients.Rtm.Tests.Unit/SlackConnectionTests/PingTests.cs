using System.Threading.Tasks;
using Moq;
using Slackbot.Net.SlackClients.Rtm.BotHelpers;
using Slackbot.Net.SlackClients.Rtm.Connections;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake;
using Slackbot.Net.SlackClients.Rtm.Connections.Monitoring;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Outbound;
using Slackbot.Net.SlackClients.Rtm.Models;
using Xunit;

namespace Slackbot.Net.SlackClients.Rtm.Tests.Unit.SlackConnectionTests
{
    public class PingTests
    {
        [Theory, AutoMoqData]
        private async Task should_send_ping(
            Mock<IWebSocketClient> webSocket, 
            Mock<IHandshakeClient> handShakeClient,
            Mock<IPingPongMonitor> pingPongMonitor,
            Mock<IMentionDetector> mentionDetector)
        {
            // given
            
            var serviceLocator = new ServiceLocator();
            var slackConnection = serviceLocator.CreateConnection(webSocket.Object, handShakeClient.Object, mentionDetector.Object, pingPongMonitor.Object);

            const string slackKey = "key-yay";

            var connectionInfo = new ConnectionInformation { SlackKey = slackKey };
            await slackConnection.Initialise(connectionInfo);
            
            // when
            await slackConnection.Ping();

            // then
            webSocket.Verify(x => x.SendMessage(It.IsAny<PingMessage>()));
        }
    }
}