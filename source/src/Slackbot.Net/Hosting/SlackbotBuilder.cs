using Microsoft.Extensions.DependencyInjection;

namespace Slackbot.Net.Hosting
{
    internal class SlackbotBuilder : ISlackbotBuilder
    {
        public SlackbotBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services
        {
            get;
        }
    }
}