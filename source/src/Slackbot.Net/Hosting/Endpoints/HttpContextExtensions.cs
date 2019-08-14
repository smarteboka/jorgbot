using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Slackbot.Net.Hosting.Web
{
    internal static class HttpContextExtensions
    {
        public static async Task WriteJsonResponse(this HttpContext context, int statusCode, string payload)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(payload);
        }
    }
}