using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Endpoints.Interactive;
using Smartbot.Utilities.Storage.Events;

namespace Smartbot.Web
{
    public class EventRsvpResponseHandler : IHandleInteractiveActions
    {
        private readonly ILogger _logger;
        private readonly IRespond _responder;
        private readonly IInvitationsStorage _storage;

        public EventRsvpResponseHandler(ILogger<EventRsvpResponseHandler> logger, IRespond responder, IInvitationsStorage storage)
        {
            _logger = logger;
            _responder = responder;
            _storage = storage;
        }

        public async Task<object> RespondToSlackInteractivePayload(IncomingInteractiveMessage incoming)
        {
            _logger.LogInformation($"ResponseUrl : {incoming.Response_Url}");
            foreach (var action in incoming.Actions)
            {
                switch (action.value)
                {
                    case RsvpValues.Attending:
                    {
                        var yesRes = await _responder.Respond(incoming.Response_Url, "Nice!");
                        await _storage.Update(action.block_id, RsvpValues.Attending);
                        return new RsvpResult
                        {
                            Rsvp = action.value
                        };
                    }
                    case RsvpValues.Maybe:
                    {
                        var maybeRes = await _responder.Respond(incoming.Response_Url, "Ok!");
                        await _storage.Update(action.block_id, RsvpValues.Maybe);
                        return new RsvpResult
                        {
                            Rsvp = action.value
                        };
                    }
                    case RsvpValues.NotAttending:
                    {
                        var noRes = await _responder.Respond(incoming.Response_Url, "Doh!");
                        await _storage.Update(action.block_id, RsvpValues.NotAttending);
                        return new RsvpResult
                        {
                            Rsvp = action.value
                        };
                    }
                }
            }

            return new RsvpResult
            {
                Rsvp = "no idea!"
            };
        }
    }

    public class RsvpResult
    {
        public string Rsvp
        {
            get;
            set;
        }
    }
}