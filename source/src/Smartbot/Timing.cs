using System;
using System.Collections.Generic;
using Cronos;

namespace Smartbot
{
    public class Timing
    {
        private const string EuropeOslo = "Europe/Oslo";
        
        public static DateTimeOffset NowInOsloTime(DateTimeOffset? nowutc = null)
        {
            var oslo = TimeZoneInfo.FindSystemTimeZoneById(EuropeOslo); 
            return TimeZoneInfo.ConvertTime(nowutc ?? DateTimeOffset.UtcNow, oslo);
        }

        public bool IsToday(DateTime date)
        {
            var nowInOslo = NowInOsloTime();
            return nowInOslo.Month == date.Month && nowInOslo.Day == date.Day;
        }

        public int CalculateAge(DateTime birthDate)
        {
            var nowInOsloTime = NowInOsloTime();
            var age = nowInOsloTime.Year - birthDate.Year;

            if (nowInOsloTime.Month < birthDate.Month || (nowInOsloTime.Month == birthDate.Month && nowInOsloTime.Day < birthDate.Day))
                age--;

            return age;
        }

        public static DateTimeOffset? GetNextOccurenceInOsloTime(string cron)
        {
            var expression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
            return expression.GetNextOccurrence(DateTimeOffset.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(EuropeOslo));
        }
        
        public static IEnumerable<DateTime> GetNextOccurences(string cron)
        {
            var expression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
            var fromUtc = DateTime.UtcNow;
            var toUtc = fromUtc.AddMonths(6);
            var nexts = expression.GetOccurrences(fromUtc,toUtc);
            return nexts;
        }
    }
}