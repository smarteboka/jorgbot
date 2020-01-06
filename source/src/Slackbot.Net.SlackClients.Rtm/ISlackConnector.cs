using System.Threading.Tasks;

namespace Slackbot.Net.SlackClients.Rtm
{
    public interface ISlackConnector
    {
        Task<ISlackConnection> Connect(string slackKey);
    }
}