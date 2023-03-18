using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Slackbot.Net.Endpoints.Authentication;
using Slackbot.Net.Endpoints.Hosting;
using Smartbot;

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
        services.AddAuthentication().AddSlackbotEvents(c => { c.SigningSecret = "123"; });
        services.AddSmartbot(context.Configuration);
    })
    .ConfigureLogging((context, configLogging) =>
    {
        configLogging
            .AddFilter("Microsoft.AspNetCore.Hosting", LogLevel.Information)
            .AddSimpleConsole(c => c.ColorBehavior = LoggerColorBehavior.Disabled)
            .AddDebug();
    }).
    Configure(app =>
    {
        // keep-alive by pinging from uptimerobot
        app.UseWhen(c => c.Request.Path == "/", a =>
        {
            a.Run(async c =>
            {
                c.Response.StatusCode = 200;
                await c.Response.WriteAsync($"Smartbot is alive {Guid.NewGuid()}");
            });
        });

        app.Map("/events", a => a.UseSlackbot(enableAuth:false));
    })

    .Build();


using (host)
{
    await host.RunAsync();
}
