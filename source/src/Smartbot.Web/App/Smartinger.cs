using Smartbot.Web.App.Utils;

namespace Smartbot.Web.App;

public record Smarting
{
    public Smarting(string name, int year, int month, int day)
    {
        Name = name;
        BirthDate = new DateOnly(year, month, day);
    }

    public DateOnly BirthDate
    {
        get;
    }

    public string Name
    {
        get;
    }
}

public class Smartinger
{
    private readonly Timing _timing;

    public List<Smarting> Smartingene = new()
    {
        new ("@mrmoen", 1982, 1, 29),
        new ("@nash", 1982, 1, 27),
        new ("@ef", 1982, 3, 16),
        new ("@thodd", 1982, 3, 18),
        new ("@glaub", 1982, 6, 7),
        new ("@tomasutnehh", 1980, 6, 23),
        new ("@trondod", 1982, 7, 5),
        new ("@tigertor", 1982, 9, 13),
        new ("@kristmel", 1982, 9, 13),
        new ("@jmrandby", 1982, 10, 17),
        new ("@jarlelin", 1982, 10, 19),
        new ("@john", 1982, 10, 21),
        new ("@mariustu", 1982, 12, 3),
        new ("@fsivertsen", 1981, 3, 21)
    };

    public Smartinger()
    {
        _timing = new Timing();
    }

    public IEnumerable<Smarting> ThatHasBirthday()
    {
        return Smartingene.Where(s => _timing.IsToday(s.BirthDate)).Select(s => s);
    }
}