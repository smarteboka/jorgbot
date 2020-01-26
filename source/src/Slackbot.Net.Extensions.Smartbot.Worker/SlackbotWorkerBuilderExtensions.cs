using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Abstractions.Hosting;
using Slackbot.Net.Abstractions.Publishers;
using Slackbot.Net.Configuration;
using Slackbot.Net.Extensions.Smartbot.SharedWorkers;
using Slackbot.Net.SlackClients.Http;
using Slackbot.Net.SlackClients.Http.Extensions;
using Smartbot.Data;
using Smartbot.Utilities;
using Smartbot.Utilities.Handlers;
using Smartbot.Utilities.Handlers._4sq;
using Smartbot.Utilities.Handlers._4sq.FourSquareServices;
using Smartbot.Utilities.RecurringActions;
using Smartbot.Utilities.SlackQuestions;
using Smartbot.Utilities.Storsdager.RecurringActions;

namespace Smartbot
{
    public static class SlackbotWorkerBuilderExtensions
    {
        public static ISlackbotWorkerBuilder AddSmartbot(this ISlackbotWorkerBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddServices(configuration);
            
            builder
                .AddRecurring<HerokuFreeTierKeepAlive>()
                .AddRecurring<Jorger>()
                .AddRecurring<HappyBirthday>()
                .AddRecurring<StorsdagMention>()
                .AddRecurring<StorsdagInvitationRecurrer>()

                .AddHandler<NesteStorsdagHandler>()
                .AddHandler<FourSquareHandler>()
                .AddHandler<OldHandler>()
                .AddHandler<UrlsSaveHandler>()
                .AddHandler<RandomSmartingHandler>()
                .AddHandler<RsvpReminder>()
                .AddHandler<LinkStatsHandler>()
                .AddHandler<LinksHandler>()
                .AddHandler<TellHandler>()
                .AddHandler<WolframHandler>()
                .AddFplBot(configuration.GetSection("smartebokafpl"))
                .BuildRecurrers();
            return builder;
        }

        private static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddData(configuration);

            services.AddSingleton<SlackChannels>();
            services.AddSingleton<Smartinger>();
            
            services.Configure<FourSquareOptions>(configuration);
            services.AddSingleton<FourSquareService>();

            services.AddSingleton<StorsdagInviter>();
            services.AddSingleton<SlackQuestionClient>();
            services.Configure<WulframOptions>(configuration);
            services.AddSingleton<IPublisher, SlackPublisher>();
            services.AddSingleton<IPublisher, LoggerPublisher>();

            services.AddSlackHttpClient(c => c.BotToken = configuration.GetValue<string>(nameof(SlackOptions.Slackbot_SlackApiKey_BotUser)));
            services.AddSlackbotOauthClient(c => c.OauthToken = configuration.GetValue<string>(nameof(SlackOptions.Slackbot_SlackApiKey_SlackApp)));
        }
    }
    
    internal class LoggerPublisher : IPublisher
    {
        private readonly ILogger<LoggerPublisher> _logger;

        public LoggerPublisher(ILogger<LoggerPublisher> logger)
        {
            _logger = logger;
        }

        public Task Publish(Notification notification)
        {
            _logger.LogInformation($"[{DateTime.UtcNow.ToLongTimeString()}] {notification.Msg}");
            return Task.CompletedTask;
        }
    }
    
    internal class SlackPublisher : IPublisher
    {
        private readonly ISlackClient _sender;

        public SlackPublisher(ISlackClient sender)
        {
            _sender = sender;
        }

        public async Task Publish(Notification notification)
        {
            await _sender.ChatPostMessage(notification.Recipient, notification.Msg);
        }
    }
}