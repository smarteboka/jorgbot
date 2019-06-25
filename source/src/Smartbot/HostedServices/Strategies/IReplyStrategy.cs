using System.Threading.Tasks;
using SlackConnector.Models;

namespace Smartbot.HostedServices.Strategies
{
    public interface IReplyStrategy
    {
        Task Handle(SlackMessage message);
        bool ShouldExecute(SlackMessage message);
    }
}