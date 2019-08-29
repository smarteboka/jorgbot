using System;
using System.Threading.Tasks;
using SlackConnector.Models;

namespace Slackbot.Net.Workers.Handlers
{
    public interface IHandleMessages
    {
        bool ShouldShowInHelp { get;}
        Tuple<string,string> GetHelpDescription();
        Task<HandleResponse> Handle(SlackMessage message);
        bool ShouldHandle(SlackMessage message);
    }
}