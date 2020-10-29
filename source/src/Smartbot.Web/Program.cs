using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Endpoints.Hosting;

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
                    services.AddSmartbot(context.Configuration);

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

                    app.UseSlackbot();
                })
             
                .Build();

   
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}