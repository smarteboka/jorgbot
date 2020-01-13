using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Smartbot.Utilities.RecurringActions;
using Slackbot.Net.Abstractions.Hosting;

namespace Smartbot.Web
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var port = environment == "Production" ? Environment.GetEnvironmentVariable("PORT") : "1337";
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:{port}")
                .UseEnvironment(environment)
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSlackbotWorker(o =>
                        {
                            o.Slackbot_SlackApiKey_BotUser = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_BotUser");
                            o.Slackbot_SlackApiKey_SlackApp = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_SlackApp");
                        })
                        .AddRecurring<HerokuFreeTierKeepAlive>();
                    
                    services.AddSlackbotEndpoints().AddSmartbotEndpoints(context.Configuration);

                })
                .ConfigureLogging((context, configLogging) =>
                {
                    configLogging
                        .AddFilter("Microsoft.AspNetCore.Hosting", LogLevel.Information)
                        .AddConsole(c => c.DisableColors = true)
                        .AddDebug();
                }).
                Configure(app =>
                {
                    // keep-alive by pinging from uptimerobot
                    app.UseWhen(c => c.Request.Path == "/", a =>
                    {
                        a.Run(c =>
                        {
                            c.Response.StatusCode = 200;
                            return Task.CompletedTask;
                        });
                    });

                    app.UseSlackbotEndpoint("/interactive");
                })
                .Build();

            var logger = host.Services.GetService<ILogger<Program>>();
            logger.LogInformation($"Using port '{port}'");

            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}