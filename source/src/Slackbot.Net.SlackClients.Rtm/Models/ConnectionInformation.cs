﻿using System.Collections.Generic;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;

namespace Slackbot.Net.SlackClients.Rtm.Models
{
    internal class ConnectionInformation
    {
        public string SlackKey { get; set; }
        public ContactDetails Self { get; set; } = new ContactDetails();
        public ContactDetails Team { get; set; } = new ContactDetails();
        public Dictionary<string, SlackUser> Users { get; set; } = new Dictionary<string, SlackUser>();
        public Dictionary<string, SlackChatHub> SlackChatHubs { get; set; } = new Dictionary<string, SlackChatHub>();
        public IWebSocketClient WebSocket { get; set; }
    }
}