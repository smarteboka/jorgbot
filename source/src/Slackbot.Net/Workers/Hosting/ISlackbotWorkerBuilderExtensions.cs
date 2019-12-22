using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions;
using Slackbot.Net.Core.Validations;
using Slackbot.Net.Workers;
using Slackbot.Net.Workers.Configuration;
using Slackbot.Net.Workers.Handlers;
using Slackbot.Net.Workers.Hosting;
using Slackbot.Net.Workers.Publishers;
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

        public static ISlackbotWorkerBuilder AddRecurring<T>(this ISlackbotWorkerBuilder builder) where T: RecurringAction
        {
            builder.Services.AddHostedService<T>();
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

        private static void AddWorkerDependencies(this ISlackbotWorkerBuilder builder)
        {
            builder.Services.AddSingleton<ISlackConnection>(ImplementationFactory);
            
            builder.Services.AddSingleton<SlackTaskClientExtensions>();
            builder.Services.AddSingleton<HandlerSelector>();
            builder.Services.AddHostedService<SlackConnectorHostedService>();
            builder.Services.AddSingleton<HelpHandler>();
        }

        private static ISlackConnection ImplementationFactory(IServiceProvider s)
        {
            var options = s.GetService<IOptions<SlackOptions>>();
            var logger = s.GetService<ILogger<SlackConnector.SlackConnector>>();
            var slackConnector = new SlackConnector.SlackConnector();
            var handlerSelector = s.GetService<HandlerSelector>();
            var connection = slackConnector.Connect(options.Value.Slackbot_SlackApiKey_BotUser).GetAwaiter().GetResult();
            connection.OnMessageReceived += handlerSelector.HandleIncomingMessage;
            if (connection.IsConnected)
                logger.LogInformation("Connected");

            return connection;
        }

    }
}