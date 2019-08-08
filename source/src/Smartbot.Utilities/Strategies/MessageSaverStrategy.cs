using System;
using System.Threading.Tasks;
using Slackbot.Net.Strategies;
using SlackConnector.Models;

namespace Smartbot.Utilities.Strategies
{
    public class MessageSaverStrategy : IReplyStrategy
    {
        public Task<HandleResponse> Handle(SlackMessage message)
        {
            return Task.FromResult(new HandleResponse("OK"));
        }

        public bool ShouldExecute(SlackMessage message)
        {
            return message.Text.StartsWith("<@UGWC87WRZ> save", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}