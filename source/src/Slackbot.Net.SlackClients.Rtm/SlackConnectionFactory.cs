using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Rtm.BotHelpers;
using Slackbot.Net.SlackClients.Rtm.Connections;
using Slackbot.Net.SlackClients.Rtm.Connections.Monitoring;
using Slackbot.Net.SlackClients.Rtm.Models;

namespace Slackbot.Net.SlackClients.Rtm
{
    internal class SlackConnectionFactory : ISlackConnectionFactory
    {
        public async Task<ISlackConnection> Create(ConnectionInformation connectionInformation)
        {
            var slackConnection = new SlackConnection(new ServiceLocator(), new MentionDetector());
            await slackConnection.Initialise(connectionInformation);
            return slackConnection;
        }
    }
}