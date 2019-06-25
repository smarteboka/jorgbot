using System.Threading.Tasks;
using SlackConnector.Models;

namespace Smartbot.HostedServices.Strategies
{
    public class DoNothingStrategy : IReplyStrategy
    {
        public Task Handle(SlackMessage message)
        {
            return Task.CompletedTask;
        }

        public bool ShouldExecute(SlackMessage message)
        {
            return !message.MentionsBot;
        }
    }
}