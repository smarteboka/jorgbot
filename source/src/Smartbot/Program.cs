using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Abstractions.Hosting;
using Slackbot.Net.Extensions.Publishers.Logger;
using Slackbot.Net.Extensions.Publishers.Slack;

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
                    services.AddSlackbotWorker(context.Configuration)
                        .AddPublisher<SlackPublisher>()
                        .AddPublisher<LoggerPublisher>()
                        .AddSmartbot(context.Configuration);

                })
                .ConfigureLogging((context, configLogging) =>
                {
                    configLogging
                        .SetMinimumLevel(LogLevel.Warning)
                        .AddFilter("Smartbot", LogLevel.Debug)
                        .AddFilter("Slackbot", LogLevel.Debug)
                        .AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information)
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