﻿using System;
using System.Threading.Tasks;
using ExpectedObjects;
using Flurl;
using Flurl.Http.Testing;
using Moq;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients.Channel;
using Slackbot.Net.SlackClients.Rtm.Connections.Models;
using Slackbot.Net.SlackClients.Rtm.Connections.Responses;
using Slackbot.Net.SlackClients.Rtm.Tests.Unit.TestExtensions;
using Xunit;

namespace Slackbot.Net.SlackClients.Rtm.Tests.Unit.Connections.Clients.Flurl
{
    public class FlurlChannelClientTests : IDisposable
    {
        private readonly HttpTest _httpTest;
        private readonly Mock<IResponseVerifier> _responseVerifierMock;
        private readonly FlurlChannelClient _channelClient;

        public FlurlChannelClientTests()
        {
            _httpTest = new HttpTest();
            _responseVerifierMock = new Mock<IResponseVerifier>();
            _channelClient = new FlurlChannelClient(_responseVerifierMock.Object);
        }

        public void Dispose()
        {
            _httpTest.Dispose();
        }

        [Fact]
        public async Task should_call_expected_url_and_return_expected_users()
        {
            // given
            const string slackKey = "I-is-another-key";

            var expectedResponse = new UsersResponse
            {
                Members = new[]
                {
                    new User { Id = "some-id-thing", IsBot = true },
                    new User { Name = "some-id-thing", Deleted = true },
                }
            };

            _httpTest.RespondWithJson(expectedResponse);

            // when
            var result = await _channelClient.GetUsers(slackKey);

            // then
            _responseVerifierMock.Verify(x => x.VerifyResponse(Looks.Like(expectedResponse)), Times.Once);
            _httpTest
                .ShouldHaveCalled(ClientConstants.SlackApiHost.AppendPathSegment(FlurlChannelClient.USERS_LIST_PATH))
                .WithQueryParamValue("token", slackKey)
                .WithQueryParamValue("presence", "1")
                .Times(1);

            result.ToExpectedObject().ShouldEqual(expectedResponse.Members);
        }
    }
}