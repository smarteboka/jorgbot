using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Oldbot.Utilities.SlackAPI.Extensions;
using SlackConnector;

namespace Oldbot.ConsoleApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOldbot(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.Configure<OldbotConfig>(configuration);
            services.AddSingleton<ISlackConnector, SlackConnector.SlackConnector>();
            services.AddSingleton<ISlackClient, SlackTaskClientExtensions>(provider =>
            {
                var config = provider.GetService<IOptions<OldbotConfig>>().Value;
                return new SlackTaskClientExtensions(config.SlackApiKeySlackApp, config.SlackApiKeyBotUser);
            });
            services.AddSingleton<OldnessValidator>();
            services.AddHostedService<OldbotHostedService>();

            return services;
        }
    }
}