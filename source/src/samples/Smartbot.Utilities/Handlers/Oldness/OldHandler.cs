using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.SlackClients;
using Slackbot.Net.SlackClients.Models.Requests.ChatPostMessage;
using Slackbot.Net.SlackClients.Models.Responses;
using SlackConnector.Models;

namespace Smartbot.Utilities.Handlers
{
    public class OldHandler : IHandleMessages
    {
        private readonly ILogger<OldHandler> _logger;
        private readonly ISlackClient _slackClient;
        private readonly ISearchClient _searchClient;

        public OldHandler(ILogger<OldHandler> logger, ISlackClient slackClient, ISearchClient searchClient)
        {
            _logger = logger;
            _slackClient = slackClient;
            _searchClient = searchClient;
        }

        public bool ShouldShowInHelp => false;

        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("<alt>", "Sjekker delte lenker for oldness");

        public async Task<HandleResponse> Handle(SlackMessage incomingMessage)
        {
            if (!string.IsNullOrEmpty(incomingMessage.Text) && !string.IsNullOrEmpty(incomingMessage.Text.Trim()))
            {
                var urls = RegexHelper.FindUrls(incomingMessage.Text);
                var responses = new List<string>();
                foreach (var url in urls)
                {
                    var res = await HandleUrl(url, incomingMessage);
                    responses.Add(res);
                }
                return new HandleResponse(responses.Aggregate((x,y) => x + "," + y));
            }
            return new HandleResponse("IGNORED");
        }

        private async Task<string> HandleUrl(string url, SlackMessage incomingMessage)
        {
            var cleansedUrl = UrlCleaner.CleanForTrackingQueryParams(url);
            cleansedUrl = cleansedUrl.TrimEnd('/');
            var searchResults = await _searchClient.SearchMessagesAsync(cleansedUrl);

            if (searchResults != null && searchResults.Messages.Matches.Any())
            {
                _logger.LogInformation(JsonConvert.SerializeObject(searchResults.Messages.Matches));
                var r = searchResults.Messages.Matches.FirstOrDefault();

                if (r == null)
                {
                    return LogHandled($"NO-URL-IN-MSG");
                }

                if (r.User == incomingMessage.User.Id)
                {
                    return LogHandled("OLD-BUT-SAME-USER-SO-IGNORING");

                }

                var threadTs = incomingMessage.Timestamp.ToString("N6");
                var reactionResponses = await AddReactions(incomingMessage.ChatHub.Id, threadTs);

                var message = $"postet av @{r.Username} for {TimeSpanExtensions.Ago(r.Ts)} siden.";


                var chatMessage = new ChatPostMessageRequest
                {
                    Channel = incomingMessage.ChatHub.Id,
                    Text = r.Permalink,
                    Parse = "full",
                    attachments = new[]
                    {
                        new Attachment {text = $":older_man: {message}", color = "#FF0000"}
                    },
                    thread_ts = threadTs
                };
                
                var response = await _slackClient.ChatPostMessage(chatMessage);

                _logger.LogInformation("Sent message. Response:" + JsonConvert.SerializeObject(response));

                var reactionResponseBody = reactionResponses.First();
                _logger.LogInformation("Sent reaction. Response:" + JsonConvert.SerializeObject(reactionResponseBody));


                return LogHandled("OLD");
            }
            return LogHandled("NEW");
        }

        private Task<Response[]> AddReactions(string channelId, string thread_ts)
        {
            var t1 = _slackClient.ReactionsAdd("older_man", channelId, thread_ts);
            var t2 = _slackClient.ReactionsAdd("older_man::skin-tone-2",channelId, thread_ts);
            var t3 = _slackClient.ReactionsAdd("older_man::skin-tone-3",channelId, thread_ts);
            var t4 = _slackClient.ReactionsAdd("older_man::skin-tone-4",channelId, thread_ts);
            var t5 = _slackClient.ReactionsAdd("older_man::skin-tone-5",channelId, thread_ts);
            return Task.WhenAll(t1, t2, t3, t4, t5);
        }


        public bool ShouldHandle(SlackMessage message)
        {
            _logger.LogInformation("Received:" + message.RawData);


            if (message.User.IsBot)
            {
                LogHandled("BOT");
                return false;
            }

            if (string.IsNullOrEmpty(message.Text))
            {
                LogHandled("IGNORED");
                return false;
            }

            var urls = RegexHelper.FindUrls(message.Text);
            return urls.Any();
        }

        private string LogHandled(string body)
        {
            _logger.LogInformation($"Treated as: {body}");
            return body;
        }
    }
}