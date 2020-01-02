using Microsoft.Extensions.DependencyInjection;

namespace Slackbot.Net.Abstractions.Hosting
{
    public class SlackbotWorkerBuilder : ISlackbotWorkerBuilder
    {
        public SlackbotWorkerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services
        {
            get;
        }
    }
}