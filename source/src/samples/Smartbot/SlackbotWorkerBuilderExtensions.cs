using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slackbot.Net.Abstractions.Hosting;
using Slackbot.Net.Extensions.Publishers.Logger;
using Slackbot.Net.Extensions.Publishers.Slack;
using Smartbot.Utilities;
using Smartbot.Utilities.Handlers;
using Smartbot.Utilities.Handlers._4sq;
using Smartbot.Utilities.Handlers._4sq.FourSquareServices;
using Smartbot.Utilities.RecurringActions;
using Smartbot.Utilities.SlackAPIExtensions;
using Smartbot.Utilities.Storage;
using Smartbot.Utilities.Storage.Events;
using Smartbot.Utilities.Storage.SlackUrls;
using Smartbot.Utilities.Storsdager.RecurringActions;

namespace Smartbot
{
    public static class ServiceCollectionExtensions
    {
        public static ISlackbotWorkerBuilder AddSmartbot(this ISlackbotWorkerBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddSingleton<SlackChannels>();
            builder.Services.AddSingleton<Smartinger>();
            builder.Services.Configure<SmartStorageOptions>(configuration);
            builder.Services.AddSingleton<SlackMessagesStorage>();

            builder.Services.AddSingleton<FourSquareService>();
            builder.Services.Configure<FourSquareOptions>(configuration);

            builder.Services.AddSingleton<IEventsStorage,EventsStorage>();
            builder.Services.AddSingleton<IInvitationsStorage,InvitationsStorage>();

            builder.Services.AddSingleton<StorsdagInviter>();
            builder.Services.AddSingleton<SlackQuestionClient>();
            
            builder.AddPublisher<SlackPublisher>()
                .AddPublisher<LoggerPublisher>()

                .AddRecurring<HerokuFreeTierKeepAlive>()
                .AddRecurring<Jorger>()
                .AddRecurring<HappyBirthday>()
                .AddRecurring<StorsdagMention>()
                .AddRecurring<StorsdagInvitationRecurrer>()

                .AddHandler<NesteStorsdagHandler>()
                .AddHandler<StorsdagerHandler>()
                .AddHandler<FourSquareHandler>()
                .AddHandler<OldHandler>()
                .AddHandler<UrlsSaveHandler>()
                .AddHandler<RandomSmartingHandler>()
                .AddHandler<RsvpReminder>()
                // .AddFplBot(context.Configuration.GetSection("smartebokafpl"))
                .BuildRecurrers();
            return builder;
        }
    }
}