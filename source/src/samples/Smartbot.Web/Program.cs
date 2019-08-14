﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Interactive;
using Smartbot.Utilities.Interactive;

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
                    services.AddSlackbotEndpoints()
                        .AddEndpointHandler<StorsdagRsvpResponseHandler>();
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