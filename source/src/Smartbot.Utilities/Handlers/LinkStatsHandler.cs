using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Handlers.Models.Rtm.MessageReceived;
using Slackbot.Net.Abstractions.Publishers;
using Slackbot.Net.SlackClients.Http;
using Smartbot.Utilities.Storage.SlackUrls;

namespace Smartbot.Utilities.Handlers
{
    public class LinkStatsHandler : IHandleMessages
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly ISlackClient _slackClient;
        private readonly SlackMessagesStorage _storage;
        private readonly ILogger<LinkStatsHandler> _logger;

        public LinkStatsHandler(IEnumerable<IPublisher> publishers, ISlackClient slackClient, SlackMessagesStorage storage, ILogger<LinkStatsHandler> logger)
        {
            _publishers = publishers;
            _slackClient = slackClient;
            _storage = storage;
            _logger = logger;
        }

        public bool ShouldShowInHelp => true;

        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("linkstats @username", "Statistikk på delte lenker for en gitt smarting");

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var strings = message.Text.Split(" ");
            if (strings.Length >= 3)
            {
                var users = await _slackClient.UsersList();
                var parsedName = strings[2];
                var usr = users.Members.FirstOrDefault(u => $"<@{u.Id}>" == parsedName);
                var smarting = parsedName;
                
                if (usr != null)
                {
                    var urlsForSmarting = await _storage.GetAllUrlsByUser(usr.Name);
                  

                    var text = $"{smarting}:\n";
            
                    if (!urlsForSmarting.Any())
                    {
                        text += $"Fant ingen lenker fra {smarting}.";
                    }
                    else
                    {
                        var domains = urlsForSmarting
                            .Select(c => new Uri(c.Url))
                            .GroupBy(GetComparableDomain);
                        
                        domains = domains.Where(o => o.Count() > 1).OrderByDescending(o => o.Count()).Take(5);

                        if (!domains.Any())
                        {
                            text += "Har ikke delt NOE fra samme domene 2 ganger! Jikes.";
                        }
                
                        foreach (var domain in domains)
                        {
                            text += $"\n•_{domain.Key}_ - {domain.Count()}";
                        } 
                    }

                    await Publish(message, text);
                }
                
                else if (parsedName == "<!channel>" || parsedName == "<!here>")
                {
                    var allUrls = await _storage.GetAllUrls();
                    var domains = allUrls
                        .Select(c => new Uri(c.Url))
                        .GroupBy(GetComparableDomain);
                        
                    domains = domains.Where(o => o.Count() > 1).OrderByDescending(o => o.Count()).Take(10);
                    var text = "Alle:";
                    foreach (var domain in domains)
                    {
                        text += $"\n•_{domain.Key}_ - {domain.Count()}";
                    } 
                    await Publish(message, text);

                }

            }
            else
            {
                var actual = await _storage.GetUniqueUsersForUrls();
                var text = $"Mangla litt input. Prøv `linkstats` og en av disse som input: {string.Join(',', actual)}";
                await Publish(message, text);

            }

            return new HandleResponse("OK");
        }

        private string GetComparableDomain(Uri u)
        {
            var s = u.Host;

            if (u.Host.StartsWith("www."))
            {
                try
                {
                    s = u.Host.Substring(4, u.Host.Length - 4);
                }
                catch (Exception)
                {
                    _logger.LogError($"{u.Host} could not be trimmed");
                }
            }
                
            
            return s;
        }

        private async Task Publish(SlackMessage message, string text)
        {
            foreach (var publisher in _publishers)
            {
                await publisher.Publish(new Notification
                {
                    Recipient = message.ChatHub.Id,
                    Msg = text
                });
            }
        }

        public bool ShouldHandle(SlackMessage message)
        {
            var sharedUrls = message.Text.Contains("linkstats", StringComparison.InvariantCultureIgnoreCase);
            return message.MentionsBot && sharedUrls;        
        }
    }
}