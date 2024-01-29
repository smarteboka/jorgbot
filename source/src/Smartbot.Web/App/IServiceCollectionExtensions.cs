using CronBackgroundServices;

using Slackbot.Net.Endpoints.Hosting;
using Slackbot.Net.SlackClients.Http.Extensions;

using Smartbot.Web.App.Bursdager;
using Smartbot.Web.App.ChatGpt;
using Smartbot.Web.App.RandomSmarting;
using Smartbot.Web.App.Storsdager.RecurringActions;
using Smartbot.Web.App.Tell;

namespace Smartbot.Web.App;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSmartbot(this IServiceCollection builder, IConfiguration configuration)
    {
        builder.AddSingleton<SlackChannels>();
        builder.AddSingleton<Smartinger>();
        builder.AddSlackHttpClient(c => c.BotToken = configuration.GetValue<string>("Slackbot_SlackApiKey_BotUser"));

        builder
            .AddRecurrer<HappyBirthday>()
            .AddRecurrer<StorsdagMention>();

        builder.AddSlackBotEvents()
            .AddShortcut<Help>()
            .AddAppMentionHandler<RandomSmartingHandler>()
            .AddAppMentionHandler<TellHandler>()
            .AddNoOpAppMentionHandler<GptHandler>()
            .AddMessageActionsHandler<GptHandler>();
        return builder;
    }
}