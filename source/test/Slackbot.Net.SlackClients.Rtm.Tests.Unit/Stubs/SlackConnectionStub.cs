using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Rtm.EventHandlers;
using Slackbot.Net.SlackClients.Rtm.Models;

namespace Slackbot.Net.SlackClients.Rtm.Tests.Unit.Stubs
{
    public class SlackConnectionStub : ISlackConnection
    {
        public string[] Aliases { get; set; }
        public IEnumerable<SlackChatHub> ConnectedDMs { get; }
        public IEnumerable<SlackChatHub> ConnectedChannels { get; }
        public IEnumerable<SlackChatHub> ConnectedGroups { get; }
        public IReadOnlyDictionary<string, SlackUser> UserNameCache { get; }
        public bool IsConnected { get; }
        public ContactDetails Team { get; }
        public ContactDetails Self { get; }
        public Task Connect(string slackKey)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public event DisconnectEventHandler OnDisconnect;
        public void RaiseOnDisconnect()
        {
            OnDisconnect?.Invoke();
        }

        public event ReconnectEventHandler OnReconnecting;
        public void RaiseOnReconnecting()
        {
            OnReconnecting?.Invoke();
        }

        public event ReconnectEventHandler OnReconnect;
        public void RaiseOnReconnect()
        {
            OnReconnect?.Invoke();
        }

        public event MessageReceivedEventHandler OnMessageReceived;
        public void RaiseOnMessageReceived()
        {
            OnMessageReceived?.Invoke(null);
        }

        public Task Ping()
        {
            throw new NotImplementedException();
        }

        public string SlackKey { get; }

        public event PongEventHandler OnPong;

        public void RaiseOnPong()
        {
            OnPong?.Invoke(DateTime.MinValue);
        }

        public Task Close()
        {
            throw new NotImplementedException();
        }
    }
}
