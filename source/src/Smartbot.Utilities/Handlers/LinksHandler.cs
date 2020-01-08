using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Handlers.Models.Rtm.MessageReceived;
using Slackbot.Net.Abstractions.Publishers;
using Smartbot.Utilities.Storage.SlackUrls;

namespace Smartbot.Utilities.Handlers
{
    public class LinksHandler : IHandleMessages
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackMessagesStorage _storage;

        public LinksHandler(IEnumerable<IPublisher> publishers, SlackMessagesStorage storage)
        {
            _publishers = publishers;
            _storage = storage;
        }

        public bool ShouldShowInHelp => true;

        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("links <tall>", "Siste delte lenker");

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var args = message.Text.Split(' ');
         

            var count = 5;
            
            if(args.Length == 3)
            {
                int parsed;
                var ok = int.TryParse(args[2], out parsed);
                if (ok && parsed <= 10)
                {
                    count = parsed;
                }
            }

            var urls = await _storage.GetLatestUrls(count);
            var lines = urls.Select(u => $"• {u}");
            var text = $"Siste {count} lenker:\n";
            text += string.Join("\n",lines);
            foreach (var publisher in _publishers)
            {
                await publisher.Publish(new Notification
                {
                    Recipient = message.ChatHub.Id,
                    Msg = text
                });
            }
            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            var sharedUrls = message.Text.Contains("links", StringComparison.InvariantCultureIgnoreCase);
            var linkStats = message.Text.Contains("linkstats", StringComparison.InvariantCultureIgnoreCase);
            return message.MentionsBot && sharedUrls && !linkStats;        
        }
    }
}