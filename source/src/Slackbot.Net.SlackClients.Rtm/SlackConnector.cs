using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Rtm.Connections;
using Slackbot.Net.SlackClients.Rtm.Connections.Models;
using Slackbot.Net.SlackClients.Rtm.Connections.Responses;
using Slackbot.Net.SlackClients.Rtm.Exceptions;
using Slackbot.Net.SlackClients.Rtm.Extensions;
using Slackbot.Net.SlackClients.Rtm.Models;

namespace Slackbot.Net.SlackClients.Rtm
{
    public class SlackConnector : ISlackConnector
    {
        public static ConsoleLoggingLevel LoggingLevel = ConsoleLoggingLevel.None;

        private readonly IConnectionFactory _connectionFactory;
        private readonly ISlackConnectionFactory _slackConnectionFactory;

        public SlackConnector() : this(new ConnectionFactory(), new SlackConnectionFactory())
        { }

        internal SlackConnector(IConnectionFactory connectionFactory, ISlackConnectionFactory slackConnectionFactory)
        {
            _connectionFactory = connectionFactory;
            _slackConnectionFactory = slackConnectionFactory;
        }

        public async Task<ISlackConnection> Connect(string slackKey)
        {
            if (string.IsNullOrEmpty(slackKey))
            {
                throw new ArgumentNullException(nameof(slackKey));
            }

            var handshakeClient = _connectionFactory.CreateHandshakeClient();
            var handshakeResponse = await handshakeClient.FirmShake(slackKey);

            if (!handshakeResponse.Ok)
            {
                throw new HandshakeException(handshakeResponse.Error);
            }

            var users = GenerateUsers(handshakeResponse.Users);

            var connectionInfo = new ConnectionInformation
            {
                SlackKey = slackKey,
                Self = new ContactDetails { Id = handshakeResponse.Self.Id, Name = handshakeResponse.Self.Name },
                Team = new ContactDetails { Id = handshakeResponse.Team.Id, Name = handshakeResponse.Team.Name },
                Users = users,
                SlackChatHubs = GetChatHubs(handshakeResponse, users.Values.ToArray()),
                WebSocket = await _connectionFactory.CreateConnectedWebSocketClient(handshakeResponse.WebSocketUrl)
            };

            var connection = await _slackConnectionFactory.Create(connectionInfo);
            return connection;
        }

        private Dictionary<string, SlackUser> GenerateUsers(User[] users)
        {
            return users.ToDictionary(user => user.Id, user => user.ToSlackUser());
        }

        private Dictionary<string, SlackChatHub> GetChatHubs(HandshakeResponse handshakeResponse, SlackUser[] users)
        {
            var hubs = new Dictionary<string, SlackChatHub>();

            foreach (Channel channel in handshakeResponse.Channels.Where(x => !x.IsArchived))
            {
                if (channel.IsMember)
                {
                    var newChannel = channel.ToChatHub();
                    hubs.Add(channel.Id, newChannel);
                }
            }

            foreach (Group group in handshakeResponse.Groups.Where(x => !x.IsArchived))
            {
                if ((group.Members ?? new string[0]).Any(x => x == handshakeResponse.Self.Id))
                {
                    var newGroup = group.ToChatHub();
                    hubs.Add(group.Id, newGroup);
                }
            }

            foreach (Im im in handshakeResponse.Ims)
            {
                hubs.Add(im.Id, im.ToChatHub(users));
            }

            return hubs;
        }
    }
}