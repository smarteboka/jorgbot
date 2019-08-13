using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Slackbot.Net.Interactive
{
    public class HttpResponder : IHttpResponder
    {
        private readonly IHandleInteractiveActions _responseHandler;

        public HttpResponder(IHandleInteractiveActions responseHandler)
        {
            _responseHandler = responseHandler;
        }

        public async Task Respond(HttpContext context)
        {
            var body = await context.Request.ReadFormAsync();
            var payload = body["payload"];
            if (string.IsNullOrEmpty(payload))
            {
                context.Response.StatusCode = 400;
                var loggerFactory = context.RequestServices.GetService<ILoggerFactory>();
                var errorLogger = loggerFactory.CreateLogger("interactive");
                errorLogger.LogError("No payload");
                await context.Response.WriteAsync("No payload");
            }
            else
            {
                var incomingInteractiveMessage = JsonConvert.DeserializeObject<IncomingInteractiveMessage>(payload);
                var handleResponse = await _responseHandler.RespondToSlackInteractivePayload(incomingInteractiveMessage);
                context.Response.Headers.Add("Content-Type", "application/json");
                await context.Response.WriteAsync(JsonConvert.SerializeObject(handleResponse));
            }
        }
    }
}