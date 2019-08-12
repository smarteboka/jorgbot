using System.Threading.Tasks;
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
        public Task<InteractiveMessageHandledResponse> RespondToSlackInteractivePayload(string body)
        {
            _logger.LogInformation(body);
            var incoming = JsonConvert.DeserializeObject<IncomingInteractiveMessage>(body);
            _logger.LogInformation($"ResponseUrl : {incoming.Response_Url}");
            return Task.FromResult(new InteractiveMessageHandledResponse
            {
                Handled = true
            });
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
        public bool Handled
        {
            get;
            set;
        }
    }
}