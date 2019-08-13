using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Slackbot.Net
{
    public class InteractiveResponseHandler
    {
        private readonly ILogger<InteractiveResponseHandler> _logger;

        public InteractiveResponseHandler(ILogger<InteractiveResponseHandler> logger)
        {
            _logger = logger;
        }

        public async Task<InteractiveMessageHandledResponse> RespondToSlackInteractivePayload(string payload)
        {
            _logger.LogInformation(payload);
            var incoming = JsonConvert.DeserializeObject<IncomingInteractiveMessage>(payload);
            _logger.LogInformation($"ResponseUrl : {incoming.Response_Url}");
            var httpClient = new HttpClient();

            var response = new InteractiveMessageHandledResponse
            {
                Text = "Nice, takk",
                Is_Ephemeral = true,
                Delete_Original = true
            };
            var serializedResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(serializedResponse, Encoding.UTF8, "application/json");
            var resp = await httpClient.PostAsync(incoming.Response_Url, content);
            var response_url_response = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("Could not reply to response_url");
                throw new Exception(response_url_response);
            }
            _logger.LogInformation(response_url_response);
            return response;
        }
    }
}