using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Smartbot.Web
{
    internal class InteractiveResponseHandler
    {
        private readonly ILogger<InteractiveResponseHandler> _logger;

        public InteractiveResponseHandler(ILogger<InteractiveResponseHandler> logger)
        {
            _logger = logger;
        }
        public async Task<InteractiveMessageHandledResponse> RespondToSlackInteractivePayload(string body)
        {
            _logger.LogInformation(body);
            var json = body.Substring(9, body.Length-9);
            var urlDecoded = HttpUtility.UrlDecode(json);
            _logger.LogInformation(urlDecoded);
            var incoming = JsonConvert.DeserializeObject<IncomingInteractiveMessage>(urlDecoded);
            _logger.LogInformation($"ResponseUrl : {incoming.Response_Url}");
            var httpClient = new HttpClient();

            var response = new InteractiveMessageHandledResponse
            {
                Text = "Ok, thxbye"
            };
            var serializedResponse = JsonConvert.SerializeObject(response);
            var content = new StringContent(serializedResponse, Encoding.UTF8);
            await httpClient.PostAsync(incoming.Response_Url, content);
            return response;
        }
    }

    internal class IncomingInteractiveMessage
    {
        public string Response_Url
        {
            get;
            set;
        }
    }

    internal class InteractiveMessageHandledResponse
    {
        public string Text
        {
            get;
            set;
        }
    }
}