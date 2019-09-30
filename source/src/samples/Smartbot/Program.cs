using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Workers.Publishers.Logger;
using Slackbot.Net.Workers.Publishers.Slack;
using Smartbot.Utilities;
using Smartbot.Utilities.RecurringActions;
using Smartbot.Utilities.Handlers;
using Smartbot.Utilities.Handlers._4sq;
using Smartbot.Utilities.Storsdager.RecurringActions;

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

                    services.AddSlackbotWorker(o =>
                        {
                            o.Slackbot_SlackApiKey_BotUser = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_BotUser");
                            o.Slackbot_SlackApiKey_SlackApp = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_SlackApp");
                        })

                        .AddPublisher<SlackPublisher>()
                        .AddPublisher<LoggerPublisher>()

                        .AddRecurring<HerokuFreeTierKeepAlive>()
                        .AddRecurring<Jorger>(c => { c.Cron = Crons.EveryDayAtNine; })
                        .AddRecurring<HappyBirthday>(c => c.Cron = Crons.EveryDayAtEight)
                        .AddRecurring<HeartBeater>(c => c.Cron = Crons.EveryDayAtSeven55)
                        .AddRecurring<StorsdagMention>(c => c.Cron = Crons.LastThursdayOfMonthCron)
                        .AddRecurring<StorsdagInvitationRecurrer>(c => c.Cron = Crons.ThirdSaturdayOfMonth)

                        .AddHandler<NesteStorsdagHandler>()
                        .AddHandler<StorsdagerHandler>()
                        .AddHandler<FourSquareHandler>()
                        .AddHandler<OldHandler>()
                        .AddHandler<UrlsSaveHandler>()
                        .AddHandler<RandomSmartingHandler>()
                        .AddHandler<RsvpReminder>();

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