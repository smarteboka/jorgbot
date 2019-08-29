using System;
using System.Threading.Tasks;
using SlackConnector.Models;

namespace Slackbot.Net.Workers.Handlers
{
    public class NoOpHandler : IHandleMessages
    {
        public bool ShouldShowInHelp
        {
            get;
        }

        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("nada", "Gjør ingenting");

        public Task<HandleResponse> Handle(SlackMessage message)
        {
            return Task.FromResult(new HandleResponse("OK"));
        }

        public bool ShouldHandle(SlackMessage message)
        {
            return !message.MentionsBot;
        }
    }
}