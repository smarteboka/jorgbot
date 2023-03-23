using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Slackbot.Net.Endpoints.Authentication;
using Slackbot.Net.Endpoints.Hosting;
using Smartbot;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var port = environment == "Production" ? Environment.GetEnvironmentVariable("PORT") : "1337";
var builder = WebApplication.CreateBuilder();
builder.WebHost.UseUrls($"http://*:{port}");
builder.Host.UseEnvironment(environment);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddAuthentication().AddSlackbotEvents(c => { c.SigningSecret = "123"; });
builder.Services.AddSmartbot(builder.Configuration);
builder.Logging
        .AddFilter("Microsoft.AspNetCore.Hosting", LogLevel.Information)
        .AddFilter("Slackbot.Net.Endpoints.Middlewares", LogLevel.Trace)
        .AddSimpleConsole(c => c.ColorBehavior = LoggerColorBehavior.Disabled)
        .AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);


var app = builder.Build();

app.MapGet("/", async c => 
{
    c.Response.StatusCode = 200;
    await c.Response.WriteAsync(Html());
});

app.Map("/events", a => a.UseSlackbot(false));

await app.RunAsync();

string Html()
{
    return
$"""
<html>
<head>
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600;700;800;900&display=swap" rel="stylesheet">
</head>
<body style="background: black; color:white;font-family: 'Inter', sans-serif; " >
    <h2 style="">Smartbot v{VersionDetails.Versions().MajorMinorPatch}</h2>
    <ul>
        <li><span style="display:inline-block;font-weight:600;width:100px">Version:</span>{VersionDetails.Versions().Informational}</li>
        <li><span style="display:inline-block;font-weight:600;width:100px">Request:</span>{Guid.NewGuid()}</li>
        <li><span style="display:inline-block;font-weight:600;width:100px">Source:</span><a
                style="color:white"
                href="https://github.com/smarteboka/smartbot/commit/{VersionDetails.Versions().Sha}">https://github.com/smarteboka/smartbot/commit/{VersionDetails.Versions().Sha}
            </a>
        </li>
    </ul>
</body>
</html>
""";
}
