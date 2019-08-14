using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Slackbot.Net;
using Slackbot.Net.Handlers;
using Slackbot.Net.Hosting;
using Slackbot.Net.Integrations.SlackAPI.Extensions;
using Slackbot.Net.Publishers;
using Slackbot.Net.Publishers.Slack;
using Slackbot.Net.Validations;
using SlackConnector;

// namespace on purpose:
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ISlackbotWorkerBuilderExtensions
    {
        public static ISlackbotWorkerBuilder AddSlackbotWorker(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureAndValidate<SlackOptions>(configuration);
            var builder = new SlackbotWorkerBuilder(services);
            builder.AddWorkerDependencies();
            return builder;
        }

        public static ISlackbotWorkerBuilder AddSlackbotWorker(this IServiceCollection services, Action<SlackOptions> action)
        {
            services.ConfigureAndValidate(action);
            var builder = new SlackbotWorkerBuilder(services);
            builder.AddWorkerDependencies();
            return builder;
        }

        public static ISlackbotWorkerBuilder AddRecurring<T>(this ISlackbotWorkerBuilder builder, Action<CronOptions> o) where T: RecurringAction
        {
            builder.Services.ConfigureAndValidate(typeof(T).ToString(), o);
            builder.Services.AddHostedService<T>();
            return builder;
        }

        public static ISlackbotWorkerBuilder AddHandler<T>(this ISlackbotWorkerBuilder builder) where T : class, IHandleMessages
        {
            builder.Services.AddSingleton<IHandleMessages, T>();
            return builder;
        }
        public static ISlackbotWorkerBuilder AddPublisher<T>(this ISlackbotWorkerBuilder builder) where T: class, IPublisher
        {
            builder.Services.AddSingleton<IPublisher, T>();
            return builder;
        }

        internal static void AddWorkerDependencies(this ISlackbotWorkerBuilder builder)
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