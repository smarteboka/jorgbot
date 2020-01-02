using Microsoft.Extensions.DependencyInjection;

namespace Slackbot.Net.Hosting
{
    public interface ISlackbotWorkerBuilder
    {
        IServiceCollection Services { get; }
    }
}