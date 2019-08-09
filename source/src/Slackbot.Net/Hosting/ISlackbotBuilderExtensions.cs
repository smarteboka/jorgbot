using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Slackbot.Net.Handlers;
using Slackbot.Net.Integrations.SlackAPI.Extensions;
using Slackbot.Net.Publishers;
using Slackbot.Net.Publishers.Slack;
using Slackbot.Net.Validations;
using SlackConnector;

// namespace on purpose:
namespace Slackbot.Net.Hosting
{
    public static class ISlackbotBuilderExtensions
    {
        public static ISlackbotBuilder AddSlackbot(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureAndValidate<SlackOptions>(configuration);
            var builder = new SlackbotBuilder(services);
            builder.AddDependencies();
            return builder;
        }

        public static ISlackbotBuilder AddSlackbot(this IServiceCollection services, Action<SlackOptions> action)
        {
            services.ConfigureAndValidate(action);
            var builder = new SlackbotBuilder(services);
            builder.AddDependencies();
            return builder;
        }

        public static ISlackbotBuilder AddRecurring<T>(this ISlackbotBuilder builder, Action<CronOptions> o) where T: RecurringAction
        {
            builder.Services.ConfigureAndValidate(typeof(T).ToString(), o);
            builder.Services.AddHostedService<T>();
            return builder;
        }

        public static ISlackbotBuilder AddHandler<T>(this ISlackbotBuilder builder) where T : class, IHandleMessages
        {
            builder.Services.AddSingleton<IHandleMessages, T>();
            return builder;
        }
        public static ISlackbotBuilder AddPublisher<T>(this ISlackbotBuilder builder) where T: class, IPublisher
        {
            builder.Services.AddSingleton<IPublisher, T>();
            return builder;
        }

        internal static void AddDependencies(this ISlackbotBuilder builder)
        {
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