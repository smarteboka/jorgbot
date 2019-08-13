using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Slackbot.Net.Interactive
{
    public interface IHttpResponder
    {
        Task Respond(HttpContext context);
    }
}