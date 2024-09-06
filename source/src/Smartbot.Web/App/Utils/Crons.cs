namespace Smartbot.Web.App.Utils;

public class Crons
{
    public const string EveryDayAtSeven55 = "0 55 7 * * *";
    public const string EveryDayAtEight = "0 0 8 * * *";
    public const string EveryDayAtNine = "0 0 9 * * *";


    // storsdag stuff:
    public const string LastWednesdayOfMonthCron = "0 0 8 * * WEDL";
    public const string ThirdSaturdayOfMonth = "0 0 10 * * 6#3";
}
