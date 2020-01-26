using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slackbot.Net.Abstractions.Hosting;
using Slackbot.Net.Configuration;
using Slackbot.Net.Extensions.Smartbot.SharedWorkers;
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
            services.AddSlackHttpClient(c => c.BotToken = configuration.GetValue<string>(nameof(SlackOptions.Slackbot_SlackApiKey_BotUser)));
        }
    }
}