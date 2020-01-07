﻿using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using SlackConnector.Connections.Models;
using SlackConnector.Connections.Responses;

namespace SlackConnector.Connections.Clients.Channel
{
    internal class FlurlChannelClient : IChannelClient
    {
        private readonly IResponseVerifier _responseVerifier;
        internal const string CHANNELS_LIST_PATH = "/api/channels.list";
        internal const string GROUPS_LIST_PATH = "/api/groups.list";
        internal const string USERS_LIST_PATH = "/api/users.list";
        
        public FlurlChannelClient(IResponseVerifier responseVerifier)
        {
            _responseVerifier = responseVerifier;
        }

        public async Task<Models.Channel[]> GetChannels(string slackKey)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(CHANNELS_LIST_PATH)
                       .SetQueryParam("token", slackKey)
                       .GetJsonAsync<ChannelsResponse>();

            _responseVerifier.VerifyResponse(response);
            return response.Channels;
        }

        public async Task<Group[]> GetGroups(string slackKey)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(GROUPS_LIST_PATH)
                       .SetQueryParam("token", slackKey)
                       .GetJsonAsync<GroupsResponse>();

            _responseVerifier.VerifyResponse(response);
            return response.Groups;
        }

        public async Task<User[]> GetUsers(string slackKey)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(USERS_LIST_PATH)
                       .SetQueryParam("token", slackKey)
                       .SetQueryParam("presence", "1")
                       .GetJsonAsync<UsersResponse>();

            _responseVerifier.VerifyResponse(response);
            return response.Members;
        }
    }
}