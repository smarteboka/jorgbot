using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Logging;

namespace JorgBot.HostedServices
{
    public abstract class CronHostedService : HostedService
    {
        private readonly ILogger<CronHostedService> _logger;

        protected CronHostedService(ILogger<CronHostedService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var cron = Cron();
            DateTime? next = null;

            do
            {
                var now = Timing.NowInOsloTime();

                if (next == null)
                {
                    next = Timing.GetNextOccurenceInOsloTime(cron);
                    _logger.LogInformation($"Next at {next.Value.ToLongDateString()} {next.Value.ToLongTimeString()}");
                }

                if (now > next)
                {
                    await Process();
                    next = Timing.GetNextOccurenceInOsloTime(cron);
                    _logger.LogInformation($"Next at {next.Value.ToLongDateString()} {next.Value.ToLongTimeString()}");
                }
                else
                {
                    // needed for graceful shutdown for some reason.
                    // 100ms chosen so it doesn't affect calculating the next
                    // cron occurence (lowest possible: every second)
                    await Task.Delay(100, stoppingToken);
                }

            } while (!stoppingToken.IsCancellationRequested);
        }

        protected abstract string Cron();

        protected abstract Task Process();
    }
}