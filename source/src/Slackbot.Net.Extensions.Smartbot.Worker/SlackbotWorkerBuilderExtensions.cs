using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slackbot.Net.Abstractions.Hosting;
using Slackbot.Net.Extensions.Smartbot.SharedWorkers;
using Smartbot.Data;
using Smartbot.Utilities;
using Smartbot.Utilities.Handlers;
using Smartbot.Utilities.Handlers._4sq;
using Smartbot.Utilities.Handlers._4sq.FourSquareServices;
using Smartbot.Utilities.RecurringActions;
using Smartbot.Utilities.SlackAPIExtensions;
using Smartbot.Utilities.SlackQuestions;
using Smartbot.Utilities.Storsdager.RecurringActions;

namespace Smartbot
{
    public static class SlackbotWorkerBuilderExtensions
    {
        public static ISlackbotWorkerBuilder AddSmartbot(this ISlackbotWorkerBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddSingleton<SlackChannels>();
            builder.Services.AddSingleton<Smartinger>();
            builder.Services.AddData(configuration);

            builder.Services.AddSingleton<FourSquareService>();
            builder.Services.Configure<FourSquareOptions>(configuration);

      

            builder.Services.AddSingleton<StorsdagInviter>();
            builder.Services.AddSingleton<SlackQuestionClient>();
            
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
                .AddFplBot(configuration.GetSection("smartebokafpl"))
                .BuildRecurrers();
            return builder;
        }
    }
}