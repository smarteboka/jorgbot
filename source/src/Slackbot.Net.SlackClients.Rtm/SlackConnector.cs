using System;
using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Rtm.Connections;
using Slackbot.Net.SlackClients.Rtm.Exceptions;

namespace Slackbot.Net.SlackClients.Rtm
{
    public class SlackConnector : ISlackConnector
    {
        public static ConsoleLoggingLevel LoggingLevel = ConsoleLoggingLevel.None;

        private readonly IServiceLocator _serviceLocator;

        public SlackConnector() : this(new ServiceLocator())
        { }

        internal SlackConnector(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public async Task<ISlackConnection> Connect(string slackKey)
        {
            if (string.IsNullOrEmpty(slackKey))
            {
                throw new ArgumentNullException(nameof(slackKey));
            }

            var handshakeClient = _serviceLocator.CreateHandshakeClient();
            var handshakeResponse = await handshakeClient.FirmShake(slackKey);

            if (!handshakeResponse.Ok)
            {
                throw new HandshakeException(handshakeResponse.Error);
            }

            var websocket = _serviceLocator.CreateConnectedWebSocketClient();
            await websocket.Connect(handshakeResponse.WebSocketUrl);
            
            var connectionInfo = ConnectionInformationMapper.CreateConnectionInformation(slackKey, handshakeResponse);

            var connection =  _serviceLocator.CreateConnection(websocket);
            await connection.Initialise(connectionInfo);
            
            return connection;
        }

      
    }
}