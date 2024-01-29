namespace Smartbot.Web.App.Utils;

public class Timing
{
    public bool IsToday(DateOnly date)
    {
        var nowInOslo = RelativeNow();
        return nowInOslo.Month == date.Month && nowInOslo.Day == date.Day;
    }

    public int CalculateAge(DateOnly birthDate)
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