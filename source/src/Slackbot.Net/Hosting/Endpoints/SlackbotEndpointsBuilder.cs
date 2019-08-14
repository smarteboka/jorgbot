using Microsoft.Extensions.DependencyInjection;

namespace Slackbot.Net.Hosting
{
    internal class SlackbotEndpointsBuilder : ISlackbotEndpointsBuilder
    {
        public SlackbotEndpointsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services
        {
            get;
        }
    }
}