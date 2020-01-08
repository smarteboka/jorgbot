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
    public class UrlForUserHandler : IHandleMessages
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackMessagesStorage _storage;

        public UrlForUserHandler(IEnumerable<IPublisher> publishers, SlackMessagesStorage storage)
        {
            _publishers = publishers;
            _storage = storage;
        }

        public bool ShouldShowInHelp => true;

        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("linkstats <slacknavn>", "Statistikk på delte lenker");

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var strings = message.Text.Split(" ");
            if (strings.Length >= 3)
            {
                var smarting = strings[2];

                var urlsForSmarting = await _storage.GetAllUrlsByUser(smarting);
                var domains = urlsForSmarting
                    .Select(c => new Uri(c.Url))
                    .GroupBy(u => u.Host);

                var text = $"{smarting}:\n";
            
                if (!domains.Any())
                {
                    var actual = await _storage.GetUniqueUsersForUrls();
                    text += $"Fant ikke {smarting}. Prøv med en av {string.Join(',', actual)}";
                }
            
                if (domains.Count() == 1)
                {
                    text += $"{domains.First().Key} - {domains.First().Count()}";
                }

                domains = domains.Where(o => o.Count() > 1).OrderByDescending(o => o.Count());
                
                foreach (var domain in domains)
                {
                    text += $"\n•_{domain.Key}_ - {domain.Count()}";
                }
            
                foreach (var publisher in _publishers)
                {
                    await publisher.Publish(new Notification
                    {
                        Recipient = message.ChatHub.Id,
                        Msg = text
                    });
                }
            }

            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            var sharedUrls = message.Text.Contains("linkstats", StringComparison.InvariantCultureIgnoreCase);
            return message.MentionsBot && sharedUrls;        
        }
    }
}