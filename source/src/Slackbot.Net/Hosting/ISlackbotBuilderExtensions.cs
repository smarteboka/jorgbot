using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Slackbot.Net;
using Slackbot.Net.Hosting;
using Slackbot.Net.Publishers;
using Slackbot.Net.Publishers.Slack;
using Slackbot.Net.Strategies;
using Slackbot.Net.Utilities.SlackAPI.Extensions;
using SlackConnector;

// namespace on purpose:
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ISlackbotBuilderExtensions
    {

        public static ISlackbotBuilder AddSlackbot(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SlackOptions>(configuration);
            var builder = new SlackbotBuilder(services);
            builder.AddDependencies();
            return builder;
        }

        public static ISlackbotBuilder AddCron<T>(this ISlackbotBuilder builder) where T: CronHostedService
        {
            builder.Services.AddHostedService<T>();
            return builder;
        }

        public static ISlackbotBuilder AddReplyStrategy<T>(this ISlackbotBuilder builder) where T : class, IReplyStrategy
        {
            builder.Services.AddSingleton<IReplyStrategy, T>();
            return builder;
        }

        internal static void AddDependencies(this ISlackbotBuilder builder)
        {
            builder.Services.AddSingleton<SlackSender>();
            builder.Services.AddSingleton<Timing>();
            builder.Services.AddSingleton<SlackChannels>();
            builder.Services.AddSingleton<IPublisher, SlackPublisher>();
            builder.Services.AddSingleton<IPublisher, ConsolePublisher>();

            builder.Services.AddSingleton<ISlackConnector, SlackConnector.SlackConnector>();
            builder.Services.AddSingleton<ISlackClient, SlackTaskClientExtensions>(provider =>
            {
                var config = provider.GetService<IOptions<SlackOptions>>().Value;
                return new SlackTaskClientExtensions(config.SmartBot_SlackApiKey_SlackApp, config.SmartBot_SlackApiKey_BotUser);
            });

            builder.Services.AddSingleton<StrategySelector>();
            builder.Services.AddHostedService<RealTimeBotHostedService>();
        }
    }
}