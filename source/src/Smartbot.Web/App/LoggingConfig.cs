using Microsoft.Extensions.Logging.Console;

namespace Smartbot.Web.App;

public static class LoggingConfig
{
    public static void Configure(this ILoggingBuilder Logging)
    {
        Logging
            .AddFilter("Microsoft.AspNetCore.Hosting", LogLevel.Information)
            .AddFilter("Slackbot.Net.Endpoints.Middlewares", LogLevel.Trace)
            .AddFilter("Slackbot.Net.SlackClients.Http", LogLevel.Trace)
            .AddSimpleConsole(c =>
            {
                c.ColorBehavior = LoggerColorBehavior.Disabled;
                c.SingleLine = true;
            })
            .AddDebug();
        Logging.SetMinimumLevel(LogLevel.Information);
    }
}