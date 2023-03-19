using CronBackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slackbot.Net.Endpoints.Hosting;
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
        public static IServiceCollection AddSmartbot(this IServiceCollection builder, IConfiguration configuration)
        {
            builder.AddServices(configuration);

            builder
                .AddRecurrer<HerokuFreeTierKeepAlive>()
                .AddRecurrer<HappyBirthday>()
                .AddRecurrer<StorsdagMention>()
                .AddRecurrer<StorsdagInvitationRecurrer>();
                

            builder.AddSlackBotEvents()
                .AddShortcut<Help>()
                .AddAppMentionHandler<NesteStorsdagHandler>()
                .AddAppMentionHandler<FourSquareHandler>()
                .AddAppMentionHandler<RandomSmartingHandler>()
                .AddAppMentionHandler<RsvpReminder>()
                .AddAppMentionHandler<TellHandler>()
                .AddAppMentionHandler<WolframHandler>()
                .AddMessageActionsHandler<MessageActionsHandler>();
            return builder;
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddData(configuration);

            services.AddSingleton<SlackChannels>();
            services.AddSingleton<Smartinger>();

            services.Configure<FourSquareOptions>(configuration);
            services.AddSingleton<FourSquareService>();

            services.AddSingleton<StorsdagInviter>();
            services.AddSingleton<SlackQuestionClient>();
            services.Configure<WulframOptions>(configuration);

            services.AddSlackHttpClient(c => c.BotToken = configuration.GetValue<string>("Slackbot_SlackApiKey_BotUser"));
            services.AddSlackbotOauthClient(c => c.OauthToken = configuration.GetValue<string>("lackbot_SlackApiKey_SlackApp"));
        }
    }
}
