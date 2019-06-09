using System;
using System.Threading.Tasks;
using JorgBot.HostedServices;
using JorgBot.HostedServices.CronServices;
using JorgBot.Publishers;
using JorgBot.Publishers.Slack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JorgBot
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
                    services.Configure<SlackOptions>(context.Configuration);
                    services.AddSingleton<SlackSender>();
                    services.AddSingleton<Smartinger>();
                    services.AddSingleton<Timing>();
                    services.AddSingleton<SlackChannels>();
                    services.AddSingleton<IPublisher, SlackPublisher>();
                    services.AddSingleton<IPublisher, ConsolePublisher>();
                    services.AddHostedService<JorgingHostedService>();
                    services.AddHostedService<BirthdayCheckerHostedService>();
                    services.AddHostedService<HeartBeatHostedService>();
                    services.AddHostedService<StorsdagsWeekHostedService>();

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