﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using Shouldly;
using Slackbot.Net.SlackClients.Rtm.Connections;
using Slackbot.Net.SlackClients.Rtm.Connections.Monitoring;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets;
using Slackbot.Net.SlackClients.Rtm.Models;
using Xunit;

namespace Slackbot.Net.SlackClients.Rtm.Tests.Unit.SlackConnectionTests
{
    public class given_valid_connection_info
    {
        [Theory, AutoMoqData]
        private void should_initialise_slack_connection(SlackConnection connection)
        {
            // given
            var info = new ConnectionInformation
            {
                Self = new ContactDetails { Id = "self-id" },
                Team = new ContactDetails { Id = "team-id" },
                Users = new Dictionary<string, SlackUser> { { "userid", new SlackUser() { Name = "userName" } } },
                SlackChatHubs = new Dictionary<string, SlackChatHub> { { "some-hub", new SlackChatHub() } },
                WebSocket = new Mock<IWebSocketClient>().Object
            };

            // when
            connection.Initialise(info).Wait();

            // then
            connection.Self.ShouldBe(info.Self);
            connection.Team.ShouldBe(info.Team);
            connection.ConnectedHubs.ShouldBe(info.SlackChatHubs);
        }

        [Theory, AutoMoqData]
        private void should_be_connected_if_websocket_is_alive(
            Mock<IWebSocketClient> webSocketClient, 
            SlackConnection connection)
        {
            // given
            var info = new ConnectionInformation
            {
                WebSocket = webSocketClient.Object
            };

            webSocketClient
                .Setup(x => x.IsAlive)
                .Returns(true);

            // when
            connection.Initialise(info).Wait();

            // then
            connection.IsConnected.ShouldBeTrue();
        }

        [Theory, AutoMoqData]
        private async Task should_initialise_ping_pong_monitor(
            [Frozen]Mock<IServiceLocator> monitoringFactory, 
            Mock<IPingPongMonitor> pingPongMonitor, 
            SlackConnection connection)
        {
            // given
            var info = new ConnectionInformation
            {
                WebSocket = new Mock<IWebSocketClient>().Object
            };

            pingPongMonitor
                .Setup(x => x.StartMonitor(It.IsAny<Func<Task>>(), It.IsAny<Func<Task>>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            monitoringFactory
                .Setup(x => x.CreatePingPongMonitor())
                .Returns(pingPongMonitor.Object);

            // when
            await connection.Initialise(info);

            // then
            pingPongMonitor.Verify(x => x.StartMonitor(It.IsNotNull<Func<Task>>(), It.IsNotNull<Func<Task>>(), TimeSpan.FromMinutes(2)), Times.Once);
        }
    }
}