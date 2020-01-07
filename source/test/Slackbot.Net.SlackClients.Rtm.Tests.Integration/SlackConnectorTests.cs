﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SlackConnector.Models;
using Xunit;
using Shouldly;

namespace SlackConnector.Tests.Integration
{
    public class SlackConnectorTests : IntegrationTest
    {
        [Fact]
        public async Task should_connect_and_stuff()
        {
            // given

            // when
            SlackConnection.OnDisconnect += SlackConnector_OnDisconnect;
            SlackConnection.OnMessageReceived += SlackConnectorOnMessageReceived;

            // then
            SlackConnection.IsConnected.ShouldBeTrue();
            //Thread.Sleep(TimeSpan.FromMinutes(5));

            // when
            await SlackConnection.Close();

            SlackConnection.IsConnected.ShouldBeFalse();
        }

        private void SlackConnector_OnDisconnect()
        {

        }

        private Task SlackConnectorOnMessageReceived(SlackMessage message)
        {
            Debug.WriteLine(message.Text);
            Console.WriteLine(message.Text);
            return Task.CompletedTask;
        }
    }
}