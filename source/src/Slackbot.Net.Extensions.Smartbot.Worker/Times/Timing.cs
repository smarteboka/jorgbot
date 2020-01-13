using System;

namespace Smartbot.Utilities.Times
{
    public class Timing
    {
        public bool IsToday(DateTime date)
        {
            var nowInOslo = RelativeNow();
            return nowInOslo.Month == date.Month && nowInOslo.Day == date.Day;
        }

        public int CalculateAge(DateTime birthDate)
        {
            var nowInOsloTime = RelativeNow();
            var age = nowInOsloTime.Year - birthDate.Year;

            if (nowInOsloTime.Month < birthDate.Month || (nowInOsloTime.Month == birthDate.Month && nowInOsloTime.Day < birthDate.Day))
                age--;

            return age;
        }
        
        public DateTimeOffset RelativeNow(DateTimeOffset? nowutc = null)
        {
            return TimeZoneInfo.ConvertTime(nowutc ?? DateTimeOffset.UtcNow, GetTimeZoneInfo());
        }
        
        private TimeZoneInfo GetTimeZoneInfo()
        {
            if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");
            }
            
            return TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        }
    }
}