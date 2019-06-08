using System;
using Microsoft.Extensions.Logging;

namespace JorgBot.HostedServices
{
    public class Timing
    {
        private readonly ILogger<Timing> _logger;

        public Timing(ILogger<Timing> logger)
        {
            _logger = logger;
        }

        public static DateTime NowUtc()
        {
            return DateTime.UtcNow;
        }

        public bool IsToday(DateTime date)
        {
            var now = NowUtc();
            var oslo = "Europe/Oslo";
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(oslo); 
            var nowInOslo = TimeZoneInfo.ConvertTime(now, timeZoneInfo);
            return nowInOslo.Month == date.Month && nowInOslo.Day == date.Day;
        }

        public int CalculateAge(DateTime birthDate)
        {
            var now = NowUtc();
            var age = now.Year - birthDate.Year;

            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                age--;

            return age;
        }
    }
}