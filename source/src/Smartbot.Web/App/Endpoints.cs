using Slackbot.Net.Endpoints.Abstractions;
using Slackbot.Net.Endpoints.Hosting;
using Slackbot.Net.Endpoints.Models.Events;
using Slackbot.Net.Endpoints.Models.Interactive.MessageActions;
using Smartbot.Web.App.ChatGpt;

namespace Smartbot.Web.App;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", async c =>
        {
            c.Response.StatusCode = 200;
            await c.Response.WriteAsync(IndexHtml.Html());
        });

        app.Map("/events", a => a.UseSlackbot(false));

        app.Map("/prompts", async (IEnumerable<INoOpAppMentions> handlers) =>
        {
            var handler = (GptHandler)handlers.First(h => h is GptHandler);
            var prompts = await handler.GeneratePrompts(new AppMentionEvent());
            return string.Join("\n\n\n", prompts.Select(p => $"{p.Role}\n {p.Content}"));
        });

        app.Map("/image", async (IEnumerable<INoOpAppMentions> handlers) =>
        {
            var handler = (GptHandler)handlers.First(h => h is GptHandler);
            await handler.CreateImage(new MessageActionInteraction
            {
                Message = new Message
                {
                    Text = "vakker natur da",
                    Thread_Ts = "1680869291.799979"

                },
                User = new User
                {
                    Id = "U0EBWMGG4",
                    Username = "smarting"
                },
                Channel = new Channel { Id = "CTECR3J6M" }
            });
        });
    }
}

public static class IndexHtml
{
    public static string Html()
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
}