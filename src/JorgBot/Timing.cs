using System;
using Cronos;

namespace JorgBot
{
    public class Timing
    {
        private const string EuropeOslo = "Europe/Oslo";
        
        public static DateTimeOffset NowInOsloTime()
        {
            var oslo = TimeZoneInfo.FindSystemTimeZoneById(EuropeOslo); 
            return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, oslo);
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

        public static DateTime? GetNextOccurenceInOsloTime(string cron)
        {
            var expression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
            var next = expression.GetNextOccurrence(NowInOsloTime(), TimeZoneInfo.FindSystemTimeZoneById(EuropeOslo));
            var nextLocalTime = next?.DateTime;
            return nextLocalTime;
        }
    }
}