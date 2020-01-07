using System;
using System.Net.Http;
using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake;
using Slackbot.Net.SlackClients.Rtm.Connections.Monitoring;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound;
using Slackbot.Net.SlackClients.Rtm.Exceptions;
using Slackbot.Net.SlackClients.Rtm.Logging;

namespace Slackbot.Net.SlackClients.Rtm
{
    public class SlackConnector : ISlackConnector
    {
        public static ConsoleLoggingLevel LoggingLevel = ConsoleLoggingLevel.None;

        private readonly IHandshakeClient _handshakeClient;
        private readonly IWebSocketClient _webSocket;
        private readonly IPingPongMonitor _pingPongMonitor;

        public SlackConnector() : this(new HandshakeClient(new HttpClient()),
            new WebSocketClientLite(new MessageInterpreter(new Logger())),
            new PingPongMonitor(new Timer(), new DateTimeKeeper()))
        { }

        internal SlackConnector(
            IHandshakeClient handshakeClient, 
            IWebSocketClient webSocket,
            IPingPongMonitor pingPongMonitor)
        {
            _handshakeClient = handshakeClient;
            _webSocket = webSocket;
            _pingPongMonitor = pingPongMonitor;
        }

        public async Task<ISlackConnection> Connect(string slackKey)
        {
            if (string.IsNullOrEmpty(slackKey))
            {
                throw new ArgumentNullException(nameof(slackKey));
            }

            var handshakeResponse = await _handshakeClient.FirmShake(slackKey);

            if (!handshakeResponse.Ok)
            {
                throw new HandshakeException(handshakeResponse.Error);
            }

            await _webSocket.Connect(handshakeResponse.WebSocketUrl);
            
            var connectionInfo = ConnectionInformationMapper.CreateConnectionInformation(slackKey, handshakeResponse);

            var connection =  new SlackConnection(_pingPongMonitor, _handshakeClient, _webSocket);
            await connection.Initialise(connectionInfo);
            
            return connection;
        }

      
    }
}