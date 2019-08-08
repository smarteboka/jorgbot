using System.Threading.Tasks;
using SlackConnector.Models;

namespace Slackbot.Net.Strategies
{
    public interface IHandleMessages
    {
        Task<HandleResponse> Handle(SlackMessage message);
        bool ShouldHandle(SlackMessage message);
    }
}