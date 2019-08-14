using Microsoft.Extensions.DependencyInjection;

namespace Slackbot.Net.Workers.Hosting
{
    internal class SlackbotWorkerBuilder : ISlackbotWorkerBuilder
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