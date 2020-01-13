using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slackbot.Net.Endpoints.Hosting;
using Smartbot.Data;
using Smartbot.Web;

namespace Smartbot
{
    public static class SlackbotEndpointsBuilderExtensions
    {
        public static ISlackbotEndpointsBuilder AddSmartbotEndpoints(this ISlackbotEndpointsBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddData(configuration);
            builder.AddEndpointHandler<EventRsvpResponseHandler>();
            return builder;
        }
    }
}