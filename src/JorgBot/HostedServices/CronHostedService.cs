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
        private DateTime? _next;

        protected CronHostedService(ILogger<CronHostedService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = Timing.NowUtc();
                var expression = CronExpression.Parse(Cron(), CronFormat.IncludeSeconds);
                
                if (_next == null)
                {
                    _next = expression.GetNextOccurrence(now).Value;
                    _logger.LogInformation($"Startup at {_next.Value.ToLongDateString()} {_next.Value.ToLongTimeString()}");
                }

                if (now > _next)
                {
                    await Process();
                    _next = expression.GetNextOccurrence(now).Value;
                    _logger.LogInformation($"Next check will be executed at {_next.Value.ToLongDateString()} {_next.Value.ToLongTimeString()}");

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