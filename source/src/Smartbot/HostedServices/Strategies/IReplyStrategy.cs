using System.Threading.Tasks;
using SlackConnector.Models;

namespace Smartbot.HostedServices.Strategies
{
    public interface IReplyStrategy
    {
        Task<HandleResponse> Handle(SlackMessage message);
        bool ShouldExecute(SlackMessage message);
    }

    public class HandleResponse
    {
        public HandleResponse(string message)
        {
            HandledMessage = message;
        }
        
        public string HandledMessage
        {
            get;
            set;
        }
    }
}