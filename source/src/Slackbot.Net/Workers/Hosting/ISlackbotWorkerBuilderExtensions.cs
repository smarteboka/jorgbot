using System;
using Microsoft.Extensions.Configuration;
using Slackbot.Net.Core.Validations;
using Slackbot.Net.SlackClients.Extensions;
using Slackbot.Net.Workers;
using Slackbot.Net.Workers.Configuration;
using Slackbot.Net.Workers.Connections;
using Slackbot.Net.Workers.Handlers;
using Slackbot.Net.Workers.Hosting;
using Slackbot.Net.Workers.Publishers;

// namespace on purpose:
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ISlackbotWorkerBuilderExtensions
    {
        public static ISlackbotWorkerBuilder AddSlackbotWorker(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureAndValidate<SlackOptions>(configuration);
            var builder = new SlackbotWorkerBuilder(services);
            builder.AddWorkerDependencies(configuration);
            return builder;
        }

        public static ISlackbotWorkerBuilder AddSlackbotWorker(this IServiceCollection services, Action<SlackOptions> action)
        {
            services.ConfigureAndValidate(action);
            var builder = new SlackbotWorkerBuilder(services);
            builder.AddWorkerDependencies(action);
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

        private static void AddWorkerDependencies(this ISlackbotWorkerBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddSlackbotClient(configuration);
            builder.Services.AddSlackbotOauthClient(configuration);
            AddWorker(builder);
        }
        
        private static void AddWorkerDependencies(this ISlackbotWorkerBuilder builder, Action<SlackOptions> configuration)
        {
            var slackOptions = new SlackOptions();
            configuration(slackOptions);
            builder.Services.AddSlackbotClient(c =>
            {
                c.BotToken = slackOptions.Slackbot_SlackApiKey_BotUser;
            });
            builder.Services.AddSlackbotOauthClient(c =>
            {
                c.OauthToken = slackOptions.Slackbot_SlackApiKey_SlackApp;
            });
            AddWorker(builder);
        }

        private static void AddWorker(ISlackbotWorkerBuilder builder)
        {
            builder.Services.AddSingleton<SlackConnectionSetup>();
            builder.Services.AddSingleton(s =>
            {
                var connection = s.GetService<SlackConnectionSetup>().Connection.Self;
                return new BotDetails
                {
                    Id = connection.Id,
                    Name = connection.Name
                };
            });
 
            builder.Services.AddSingleton<HandlerSelector>();
            builder.Services.AddHostedService<SlackConnectorHostedService>();
        }

   
    }
}