using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net;

namespace Smartbot.Utilities.RecurringActions
{
    public class HerokuFreeTierKeepAlive : RecurringAction
    {

        public HerokuFreeTierKeepAlive(ILogger<HerokuFreeTierKeepAlive> logger): base( "0 */10 * * * *",logger)
        {

        }

        public override Task Process()
        {
            Logger.LogInformation("Keep process alive");
            return Task.CompletedTask;
        }
    }
}