using Slackbot.Net.Endpoints.Authentication;
using Smartbot.Web.App;


var builder = WebApplication.CreateBuilder(args);
// See fly.toml for the PORT environment variable & services > internal_port
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var port = environment == "Production" ? Environment.GetEnvironmentVariable("PORT") : "1337";
builder.WebHost.UseUrls($"http://*:{port}");
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddAuthentication().AddSlackbotEvents(c => { c.SigningSecret = "123"; });
builder.Services.AddSmartbot(builder.Configuration);
builder.Logging.Configure();

var app = builder.Build();
app.MapEndpoints();

await app.RunAsync();