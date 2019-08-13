using System.Threading.Tasks;

namespace Slackbot.Net.Interactive
{
    public interface IHandleInteractiveActions
    {
        Task<object> RespondToSlackInteractivePayload(IncomingInteractiveMessage incoming);
    }
}