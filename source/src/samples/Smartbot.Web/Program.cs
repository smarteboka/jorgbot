﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                    services.AddRouting();
                    services.AddSingleton<InteractiveResponseHandler>();
                })
                .ConfigureLogging((context, configLogging) =>
                {
                    configLogging
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddConsole(c => c.DisableColors = true)
                        .AddDebug();
                }).
                Configure(app =>
                {
                    app.UseRouter(r => r.MapGet("/", context => context.Response.WriteAsync($"Hi, Slack!")));
                    app.UseRouter(r => r.MapPost("/interactive", async context =>
                    {
                        var body = await context.Request.Body.ReadAsStringAsync();
                        var responseHandler = context.RequestServices.GetService<InteractiveResponseHandler>();
                        var handleResponse = await responseHandler.RespondToSlackInteractivePayload(body);
                        context.Response.Headers.Add("Content-Type", "application/json");
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(handleResponse));
                    }));
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