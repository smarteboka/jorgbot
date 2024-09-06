using Smartbot.Web.App.Utils;

namespace Smartbot.Web.App.Bursdager;

public class HappyBirthday : IRecurringAction
{
    private readonly Smartinger _smartinger;
    private readonly SlackChannels _channels;
    private readonly ISlackClient _client;
    private readonly Timing _timing;

    public HappyBirthday(Smartinger smartinger, SlackChannels channels, ISlackClient client)
    {
        _smartinger = smartinger;
        _channels = channels;
        _client = client;
        _timing = new Timing();
    }

    public async Task Process(CancellationToken token)
    {
        foreach (var smarting in _smartinger.ThatHasBirthday())
        {
            await _client.ChatPostMessage(_channels.SmartebokaChannel, $"Idag jazzulerer vi {smarting.Name} med {_timing.CalculateAge(smarting.BirthDate)}-årsdagen!");
        }
    }

    public string Cron => Crons.EveryDayAtEight;
}