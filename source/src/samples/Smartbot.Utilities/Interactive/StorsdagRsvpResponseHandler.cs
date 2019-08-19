using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Endpoints.Interactive;

namespace Smartbot.Utilities.Interactive
{
    public class StorsdagRsvpResponseHandler : IHandleInteractiveActions
    {
        private readonly ILogger _logger;
        private readonly IRespond _responder;

        public StorsdagRsvpResponseHandler(ILogger<StorsdagRsvpResponseHandler> logger, IRespond responder)
        {
            _logger = logger;
            _responder = responder;
        }

        public async Task<object> RespondToSlackInteractivePayload(IncomingInteractiveMessage incoming)
        {
            LoggerExtensions.LogInformation(_logger, $"ResponseUrl : {incoming.Response_Url}");
            if (incoming.Actions.Any(a => a.value == "deltar"))
            {
                var yesRes = await _responder.Respond(incoming.Response_Url, "Nice!");

                return new RsvpResult
                {
                    Attending = true
                };
            }
            else
            {
                var noRes = await _responder.Respond(incoming.Response_Url, "Ok :/");

                return new RsvpResult
                {
                    Attending = false
                };
            }
        }
    }

    public class RsvpResult
    {
        public bool Attending
        {
            get;
            set;
        }
    }
}