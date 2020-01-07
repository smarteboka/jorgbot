using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using Shouldly;
using Slackbot.Net.SlackClients.Rtm.BotHelpers;
using Slackbot.Net.SlackClients.Rtm.Connections;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound;
using Slackbot.Net.SlackClients.Rtm.Models;
using Slackbot.Net.SlackClients.Rtm.Tests.Unit.TestExtensions;
using Xunit;

namespace Slackbot.Net.SlackClients.Rtm.Tests.Unit.SlackConnectionTests.InboundMessageTests
{
    public class ChatMessageTests
    {
        [Theory, AutoMoqData]
        private async Task should_raise_event(
            Mock<IWebSocketClient> webSocket, Mock<IMentionDetector> mentionDetector)
        {
            var serviceLocator = new ServiceLocator();
            var slackConnection = serviceLocator.CreateConnection(webSocket.Object, mentionDetector.Object);

            // given
            var connectionInfo = new ConnectionInformation
            {
                Users = { { "userABC", new SlackUser { Id = "userABC", Name = "i-have-a-name" } } },
            };
            await slackConnection.Initialise(connectionInfo);

            var inboundMessage = new ChatMessage
            {
                User = "userABC",
                MessageType = MessageType.Message,
                Text = "amazing-text",
                RawData = "I am raw data yo",
                MessageSubType = MessageSubType.bot_message
            };

            SlackMessage receivedMessage = null;
            slackConnection.OnMessageReceived += message =>
            {
                receivedMessage = message;
                return Task.CompletedTask;
            };

            // when
            webSocket.Raise(x => x.OnMessage += null, null, inboundMessage);

            // then
            receivedMessage.ShouldLookLike(new SlackMessage
            {
                Text = "amazing-text",
                User = new SlackUser { Id = "userABC", Name = "i-have-a-name" },
                RawData = inboundMessage.RawData,
                MessageSubType = SlackMessageSubType.BotMessage,
                Files = Enumerable.Empty<SlackFile>()
        });
        }

        [Theory, AutoMoqData]
        private async Task should_raise_event_given_user_information_is_missing_from_cache(
            Mock<IWebSocketClient> webSocket, 
            Mock<IMentionDetector> mentionDetector)
        {
            // given
            
            var serviceLocator = new ServiceLocator();
            var slackConnection = serviceLocator.CreateConnection(webSocket.Object, mentionDetector.Object);

            var connectionInfo = new ConnectionInformation();
            
            await slackConnection.Initialise(connectionInfo);

            var inboundMessage = new ChatMessage
            {
                User = "userABC",
                MessageType = MessageType.Message
            };

            SlackMessage receivedMessage = null;
            slackConnection.OnMessageReceived += message =>
            {
                receivedMessage = message;
                return Task.CompletedTask;
            };

            // when
            webSocket.Raise(x => x.OnMessage += null, null, inboundMessage);

            // then
            receivedMessage.ShouldLookLike(new SlackMessage
            {
                User = new SlackUser { Id = "userABC", Name = string.Empty },
                Files = Enumerable.Empty<SlackFile>()
        });
        }

        [Theory, AutoMoqData]
        private async Task should_not_raise_message_event_given_incorrect_message_type(
            Mock<IWebSocketClient> webSocket, 
            SlackConnection slackConnection)
        {
            // given
            var connectionInfo = new ConnectionInformation();
            await slackConnection.Initialise(connectionInfo);

            var inboundMessage = new ChatMessage { MessageType = MessageType.Unknown };

            bool messageRaised = false;
            slackConnection.OnMessageReceived += message =>
            {
                messageRaised = true;
                return Task.CompletedTask;
            };

            // when
            webSocket.Raise(x => x.OnMessage += null, null, inboundMessage);

            // then
            messageRaised.ShouldBeFalse();
        }

        [Theory, AutoMoqData]
        private async Task should_not_raise_message_event_given_null_message(
            Mock<IWebSocketClient> webSocket, 
            SlackConnection slackConnection)
        {
            // given
            var connectionInfo = new ConnectionInformation();
            await slackConnection.Initialise(connectionInfo);

            bool messageRaised = false;
            slackConnection.OnMessageReceived += message =>
            {
                messageRaised = true;
                return Task.CompletedTask;
            };

            // when
            webSocket.Raise(x => x.OnMessage += null, null, null);

            // then
            messageRaised.ShouldBeFalse();
        }

        [Theory, AutoMoqData]
        private async Task should_return_expected_channel_info(
            Mock<IWebSocketClient> webSocket, 
            Mock<IMentionDetector> mentionDetector)
        {
            // given
            
            var serviceLocator = new ServiceLocator();
            var slackConnection = serviceLocator.CreateConnection(webSocket.Object, mentionDetector.Object);

            var connectionInfo = new ConnectionInformation
            {
                SlackChatHubs = { { "channelId", new SlackChatHub { Id = "channelId", Name = "NaMe23" } } },
            };
            await slackConnection.Initialise(connectionInfo);

            var inboundMessage = new ChatMessage
            {
                Channel = connectionInfo.SlackChatHubs.First().Key,
                MessageType = MessageType.Message,
                User = "irmBrady"
            };

            SlackMessage receivedMessage = null;
            slackConnection.OnMessageReceived += message =>
            {
                receivedMessage = message;
                return Task.CompletedTask;
            };

            // when
            webSocket.Raise(x => x.OnMessage += null, null, inboundMessage);

            // then
            SlackChatHub expected = connectionInfo.SlackChatHubs.First().Value;
            receivedMessage.ChatHub.ShouldBe(expected);
        }

        [Theory, AutoMoqData]
        private async Task should_detect_bot_is_mentioned_in_message(
            [Frozen]Mock<IMentionDetector> mentionDetector, 
            Mock<IWebSocketClient> webSocket)
        {
            var serviceLocator = new ServiceLocator();
            var slackConnection = serviceLocator.CreateConnection(webSocket.Object, mentionDetector.Object);
            // given
            var connectionInfo = new ConnectionInformation
            {
                Self = new ContactDetails { Id = "self-id", Name = "self-name" },
            };
            await slackConnection.Initialise(connectionInfo);

            var inboundMessage = new ChatMessage
            {
                MessageType = MessageType.Message,
                Text = "please send help... :-p",
                User = "lalala"
            };

            mentionDetector
                .Setup(x => x.WasBotMentioned(connectionInfo.Self.Name, connectionInfo.Self.Id, inboundMessage.Text))
                .Returns(true);

            SlackMessage receivedMessage = null;
            slackConnection.OnMessageReceived += message => { receivedMessage = message; return Task.CompletedTask; };

            // when
            webSocket.Raise(x => x.OnMessage += null, null, inboundMessage);

            // then
            receivedMessage.MentionsBot.ShouldBeTrue();
        }

        [Theory, AutoMoqData]
        private async Task should_not_raise_message_event_given_message_from_self(
            Mock<IWebSocketClient> webSocket, 
            SlackConnection slackConnection)
        {
            // given
            var connectionInfo = new ConnectionInformation
            {
                Self = new ContactDetails { Id = "self-id", Name = "self-name" },
            };
            await slackConnection.Initialise(connectionInfo);

            var inboundMessage = new ChatMessage
            {
                MessageType = MessageType.Message,
                User = connectionInfo.Self.Id
            };

            bool messageRaised = false;
            slackConnection.OnMessageReceived += message =>
            {
                messageRaised = true;
                return Task.CompletedTask;
            };

            // when
            webSocket.Raise(x => x.OnMessage += null, null, inboundMessage);

            // then
            messageRaised.ShouldBeFalse();
        }

        [Theory, AutoMoqData]
        private async Task should_not_raise_event_given_message_is_missing_user_information(
            Mock<IWebSocketClient> webSocket, 
            SlackConnection slackConnection)
        {
            // given
            var connectionInfo = new ConnectionInformation();
            await slackConnection.Initialise(connectionInfo);

            var inboundMessage = new ChatMessage
            {
                MessageType = MessageType.Message,
                User = null
            };

            bool messageRaised = false;
            slackConnection.OnMessageReceived += message =>
            {
                messageRaised = true;
                return Task.CompletedTask;
            };

            // when
            webSocket.Raise(x => x.OnMessage += null, null, inboundMessage);

            // then
            messageRaised.ShouldBeFalse();
        }

        [Theory, AutoMoqData]
        private async Task should_not_raise_exception(
            Mock<IWebSocketClient> webSocket, 
            SlackConnection slackConnection)
        {
            // given
            var connectionInfo = new ConnectionInformation();
            await slackConnection.Initialise(connectionInfo);

            var inboundMessage = new ChatMessage
            {
                MessageType = MessageType.Message,
                User = "lalala"
            };

            slackConnection.OnMessageReceived += message => throw new Exception("EMPORER OF THE WORLD");

            // when & then (does not throw)
            webSocket.Raise(x => x.OnMessage += null, null, inboundMessage);
        }
    }
}
