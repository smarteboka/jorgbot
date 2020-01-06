using System.Threading.Tasks;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Clients.Channel
{
    internal interface IChannelClient
    {
        Task<Models.Channel[]> GetChannels(string slackKey);

        Task<Models.Group[]> GetGroups(string slackKey);

        Task<Models.User[]> GetUsers(string slackKey);
    }
}