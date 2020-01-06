using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Slackbot.Net.SlackClients.Rtm.Connections.Responses;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Clients.Handshake
{
    internal class HandshakeClient : IHandshakeClient
    {
        private readonly HttpClient _httpClient;
        private readonly IResponseVerifier _responseVerifier;
        internal const string HANDSHAKE_PATH = "/api/rtm.start";

        public HandshakeClient(HttpClient httpClient, IResponseVerifier responseVerifier)
        {
            _httpClient = httpClient;
            _responseVerifier = responseVerifier;
        }

        public async Task<HandshakeResponse> FirmShake(string slackKey)
        {
            var uri = $"{ClientConstants.SlackApiHost}/{HANDSHAKE_PATH}?token={slackKey}";
            var httpResponse = await _httpClient.GetAsync(uri);
            var content = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<HandshakeResponse>(content);
            _responseVerifier.VerifyResponse(response);
            return response;
        }
    }
}