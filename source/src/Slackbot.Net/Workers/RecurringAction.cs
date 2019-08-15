using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net.Workers.Configuration;

namespace Slackbot.Net.Workers
{
    public abstract class RecurringAction : BackgroundService
    {
        protected readonly Timing Timing;
        protected readonly ILogger<RecurringAction> Logger;

        protected RecurringAction(IOptionsSnapshot<CronOptions> options, ILogger<RecurringAction> logger)
        {
            CronOptions cronOptions = options.Get(GetType().ToString());
            Timing = new Timing();
            Timing.SetTimeZone(cronOptions.TimeZoneId);
            Logger = logger;
            Cron = cronOptions.Cron;
            Logger.LogDebug($"Using {Cron} and timezone '{Timing.TimeZoneInfo.Id}. The time in this timezone: {Timing.RelativeNow()}'");
        }

        protected string Cron;

        public abstract Task Process();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var cron = Cron;
            DateTimeOffset? next = null;

            do
            {
                var now = Timing.RelativeNow();

                if (next == null)
                {
                    next = Timing.GetNextOccurenceInRelativeTime(cron);

                    var upcoming = Timing.GetNextOccurences(cron);
                    var uText = upcoming.Select(u => $"{u.ToLongDateString()} {next.Value.DateTime.ToLongTimeString()}").Take(10);
                    Logger.LogInformation($"Next at {next.Value.DateTime.ToLongDateString()} {next.Value.DateTime.ToLongTimeString()}\n" +
                                           $"Upcoming:\n{uText.Aggregate((x,y) => x + "\n" + y)}");
                }

                if (now > next)
                {
                    await Process();
                    next = Timing.GetNextOccurenceInRelativeTime(cron);
                    Logger.LogInformation($"Next at {next.Value.DateTime.ToLongDateString()} {next.Value.DateTime.ToLongTimeString()}");
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

    }
}