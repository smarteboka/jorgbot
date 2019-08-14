using Microsoft.Extensions.DependencyInjection;

namespace Slackbot.Net.Hosting
{
    public interface ISlackbotEndpointsBuilder
    {
        IServiceCollection Services { get; }
    }
}