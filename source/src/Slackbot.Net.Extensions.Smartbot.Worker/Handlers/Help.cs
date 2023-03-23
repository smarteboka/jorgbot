using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Endpoints.Abstractions;
using Slackbot.Net.Endpoints.Models.Events;
using Slackbot.Net.SlackClients.Http;

namespace Smartbot.Utilities.Handlers
{
    public class Help : IShortcutAppMentions
    {
        private readonly IEnumerable<IHandleAppMentions> _handlers;
        private readonly ISlackClient _slackClient;

        public Help(IEnumerable<IHandleAppMentions> allHandlers, ISlackClient slackClient)
        {
            _handlers = allHandlers;
            _slackClient = slackClient;
        }

        public async Task Handle(EventMetaData eventMetadata, AppMentionEvent appMentioned)
        {
            var text = _handlers.Select(handler => handler.GetHelpDescription()).Where(s => !string.IsNullOrEmpty(s.HandlerTrigger)).Aggregate("", (current, helpDescription) => current + $"\n• `{helpDescription.HandlerTrigger}` : _{helpDescription.Description}_");
            await _slackClient.ChatPostMessage(appMentioned.Channel, text);
        }

        public bool ShouldShortcut(AppMentionEvent appMention) => appMention.Text.Contains("zhelp");
    }
}
