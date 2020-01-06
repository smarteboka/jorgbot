using System.Threading.Tasks;
using Slackbot.Net.SlackClients.Rtm.Models;

namespace Slackbot.Net.SlackClients.Rtm
{
    internal interface ISlackConnectionFactory
    {
        Task<ISlackConnection> Create(ConnectionInformation connectionInformation);
    }
}