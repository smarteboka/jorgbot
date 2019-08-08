using System;
using Microsoft.Extensions.Configuration;
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

        public static ISlackbotBuilder AddSlackbot(this IServiceCollection services, Action<SlackOptions> action)
        {
            services.Configure(action);
            var builder = new SlackbotBuilder(services);
            builder.AddDependencies();
            return builder;
        }

        public static ISlackbotBuilder AddRecurring<T>(this ISlackbotBuilder builder, Action<CronOptions> o) where T: RecurringAction
        {
            builder.Services.Configure(typeof(T).ToString(), o);
            builder.Services.AddHostedService<T>();
            return builder;
        }

        public static ISlackbotBuilder AddHandler<T>(this ISlackbotBuilder builder) where T : class, IHandleMessages
        {
            builder.Services.AddSingleton<IHandleMessages, T>();
            return builder;
        }

        public static ISlackbotBuilder AddSlackPublisher(this ISlackbotBuilder builder)
        {
            builder.Services.AddSingleton<IPublisher, SlackPublisher>();
            return builder;
        }

        public static ISlackbotBuilder AddPublisher<T>(this ISlackbotBuilder builder) where T: class, IPublisher
        {
            builder.Services.AddSingleton<IPublisher, T>();
            return builder;
        }

        internal static void AddDependencies(this ISlackbotBuilder builder)
        {

            builder.Services.AddSingleton<Timing>();
            builder.Services.AddSingleton<SlackChannels>();
            builder.Services.AddSingleton<SlackSender>();

            builder.Services.AddSingleton<ISlackConnector, SlackConnector.SlackConnector>();
            builder.Services.AddSingleton<ISlackClient, SlackTaskClientExtensions>(provider =>
            {
                var config = provider.GetService<IOptions<SlackOptions>>().Value;
                return new SlackTaskClientExtensions(config.Slackbot_SlackApiKey_SlackApp, config.Slackbot_SlackApiKey_BotUser);
            });

            builder.Services.AddSingleton<HandlerSelector>();
            builder.Services.AddHostedService<SlackConnectorHostedService>();
        }
    }
}