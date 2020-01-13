using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Abstractions.Handlers;

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

        public Task Process()
        {
            _logger.LogInformation("Keep process alive");
            return Task.CompletedTask;
        }

        public string Cron { get; }
    }
}