using Microsoft.AspNetCore.Authentication;

using Slackbot.Net.Endpoints.Authentication;
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
        builder.AddAuthentication().AddSlackbotEvents(c => { c.SigningSecret = "123"; });
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

    public static void MapSmartbot(this WebApplication app)
    {
        app.Map("/events", a => a.UseSlackbot(false));
    }
}

public class Yolo : IAuthenticationRequestHandler
{
    public Task<bool> HandleRequestAsync()
    {
        throw new NotImplementedException();
    }

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        throw new NotImplementedException();
    }

    public Task<AuthenticateResult> AuthenticateAsync()
    {
        throw new NotImplementedException();
    }

    public Task ChallengeAsync(AuthenticationProperties properties)
    {
        throw new NotImplementedException();
    }

    public Task ForbidAsync(AuthenticationProperties properties)
    {
        throw new NotImplementedException();
    }
}