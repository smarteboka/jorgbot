using Microsoft.Extensions.DependencyInjection;

namespace Slackbot.Net.Workers.Hosting
{
    public interface ISlackbotWorkerBuilder
    {
        IServiceCollection Services { get; }
    }
}