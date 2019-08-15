using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net;
using Slackbot.Net.Workers;
using Slackbot.Net.Workers.Configuration;
using Slackbot.Net.Workers.Publishers;

namespace Smartbot.Utilities.RecurringActions
{
    public class HerokuFreeTierKeepAlive : RecurringAction
    {
        public HerokuFreeTierKeepAlive(ILogger<HerokuFreeTierKeepAlive> logger, IOptionsSnapshot<CronOptions> options)
            : base(options,logger)
        {

        }

        public override Task Process()
        {
            Logger.LogInformation("KeepAlive");
            return Task.CompletedTask;
        }
    }
}