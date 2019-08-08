using System.Threading.Tasks;
using SlackConnector.Models;

namespace Slackbot.Net.Strategies
{
    public class DoNothingStrategy : IReplyStrategy
    {
        public Task<HandleResponse> Handle(SlackMessage message)
        {
            return Task.FromResult(new HandleResponse("OK"));
        }

        public bool ShouldExecute(SlackMessage message)
        {
            return !message.MentionsBot;
        }
    }
}