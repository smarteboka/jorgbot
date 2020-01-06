using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Slackbot.Net.SlackClients.Rtm.Connections.Responses;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake
{
    internal class FlurlHandshakeClient : IHandshakeClient
    {
        private readonly IResponseVerifier _responseVerifier;
        internal const string HANDSHAKE_PATH = "/api/rtm.start";

        public FlurlHandshakeClient(IResponseVerifier responseVerifier)
        {
            _responseVerifier = responseVerifier;
        }

        public async Task<HandshakeResponse> FirmShake(string slackKey)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(HANDSHAKE_PATH)
                       .SetQueryParam("token", slackKey)
                       .GetJsonAsync<HandshakeResponse>();

            _responseVerifier.VerifyResponse(response);
            return response;
        }
    }
}