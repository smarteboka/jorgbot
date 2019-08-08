using System;
using System.Threading.Tasks;
using Slackbot.Net.Strategies;
using SlackConnector.Models;

namespace Smartbot.Utilities.Handlers
{
    public class MessageSaverReplier : IHandleMessages
    {
        public Task<HandleResponse> Handle(SlackMessage message)
        {
            return Task.FromResult(new HandleResponse("OK"));
        }

        public bool ShouldHandle(SlackMessage message)
        {
            return message.Text.StartsWith("<@UGWC87WRZ> save", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}