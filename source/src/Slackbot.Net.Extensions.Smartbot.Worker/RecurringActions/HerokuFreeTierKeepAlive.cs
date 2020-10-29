using System.Threading;
using System.Threading.Tasks;
using CronBackgroundServices;
using Microsoft.Extensions.Logging;

namespace Slackbot.Net.Extensions.Smartbot.SharedWorkers
{
    public class HerokuFreeTierKeepAlive : IRecurringAction
    {
        private readonly ILogger<HerokuFreeTierKeepAlive> _logger;

        public HerokuFreeTierKeepAlive(ILogger<HerokuFreeTierKeepAlive> logger)
        {
            _logger = logger;
            Cron = "0 */10 * * * *";
        }

        public Task Process(CancellationToken token)
        {
            _logger.LogInformation("Keep process alive");
            return Task.CompletedTask;
        }

        public string Cron { get; }
    }
}