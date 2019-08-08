using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Smartbot.Utilities;
using Smartbot.Utilities.FourSquareServices;
using Smartbot.Utilities.HostedServices;
using Smartbot.Utilities.Storage;
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
                    services.AddSingleton<Smartinger>();
                    services.Configure<SmartStorageOptions>(context.Configuration);
                    services.AddSingleton<SlackMessagesStorage>();

                    services.AddSingleton<FourSquareService>();
                    services.Configure<FourSquareOptions>(context.Configuration);

                    services.AddSlackbot(context.Configuration)

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