using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Publishers;
using Smartbot.Utilities.HostedServices;
using Smartbot.Utilities.Strategies;

namespace Smartbot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development")
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSmartbot(context.Configuration);


                    services.AddSlackbot(o =>
                        {
                            o.Slackbot_SlackApiKey_BotUser = Environment.GetEnvironmentVariable("SmartBot_SlackApiKey_BotUser");
                            o.Slackbot_SlackApiKey_SlackApp = Environment.GetEnvironmentVariable("SmartBot_SlackApiKey_SlackApp");
                        })

                        .AddSlackPublisher()
                        .AddPublisher<LoggerPublisher>()

                        .AddRecurring<JorgingHostedService>(c => c.Cron = "0 55 7 * * *")
                        .AddRecurring<BirthdayCheckerHostedService>(c => c.Cron = "0 0 8 * * *")
                        .AddRecurring<HeartBeatHostedService>(c => c.Cron = "0 55 7 * * *")
                        .AddRecurring<StorsdagsWeekHostedService>(c => c.Cron = "0 0 8 * * THUL")

                        .AddHandler<NesteStorsdagHandler>()
                        .AddHandler<StorsdagerHandler>()
                        .AddHandler<FourSquareHandler>()
                        .AddHandler<OldHandler>()
                        .AddHandler<UrlsSaveHandler>();

                })
                .ConfigureLogging((context, configLogging) =>
                {
                    configLogging
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddConsole(c => c.DisableColors = true)
                        .AddDebug();
                })
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}