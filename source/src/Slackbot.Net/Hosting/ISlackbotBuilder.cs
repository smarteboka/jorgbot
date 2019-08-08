using Microsoft.Extensions.DependencyInjection;

namespace Slackbot.Net.Hosting
{
    public interface ISlackbotBuilder
    {
        IServiceCollection Services { get; }
    }
}