using System.Threading;
using System.Threading.Tasks;
using CronBackgroundServices;
using Slackbot.Net.SlackClients.Http;

namespace Smartbot.Utilities.Storsdager.RecurringActions
{
    public class StorsdagMention : IRecurringAction
    {
        private readonly ISlackClient _client;
        private readonly SlackChannels _channels;
    
        public StorsdagMention(ISlackClient client,
            SlackChannels channels)
        {
            _client = client;
            _channels = channels;
        }
    
        public async Task Process(CancellationToken token)
        {
            await _client.ChatPostMessage(_channels.SmartebokaChannel, "Storsdag!");
        }

        public string Cron => Crons.LastThursdayOfMonthCron;
    }
}