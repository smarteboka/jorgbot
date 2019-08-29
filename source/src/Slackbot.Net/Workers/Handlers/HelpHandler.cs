using Slackbot.Net.Workers.Publishers;
using SlackConnector.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Slackbot.Net.Workers.Handlers
{
    public class HelpHandler
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly IEnumerable<IHandleMessages> _handlers;

        public HelpHandler(IEnumerable<IPublisher> publishers, IEnumerable<IHandleMessages> handlers)
        {
            _publishers = publishers;
            _handlers = handlers;
        }

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var text = _handlers.Where(handler => handler.ShouldShowInHelp)
                .Select(handler => handler.GetHelpDescription())
                .Aggregate("*HALP:*", (current, helpDescription) => current + $"\n• `{helpDescription.Item1}` : _{helpDescription.Item2}_");

            var helpText = new Notification
            {
                Recipient = message.ChatHub.Id,
                Msg = text
            };

            foreach(var publisher in _publishers)
            {
                await publisher.Publish(helpText);
            }
            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            return message.MentionsBot && message.Text.Contains("help");
        }
    }
}