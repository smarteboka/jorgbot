using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackAPI;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions.Models;
using Slackbot.Net.Workers.Handlers;
using SlackConnector.Models;
using SearchSort = SlackAPI.SearchSort;
using SearchSortDirection = SlackAPI.SearchSortDirection;

namespace Smartbot.Utilities.Handlers
{
    public class OldHandler : IHandleMessages
    {
        private readonly ILogger<OldHandler> _logger;
        private readonly SlackTaskClientExtensions _slackClient;

        public OldHandler(ILogger<OldHandler> logger, SlackTaskClientExtensions slackClient)
        {
            _logger = logger;
            _slackClient = slackClient;
        }

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
            var searchResults = await _slackClient.SearchMessagesAsync(cleansedUrl, SearchSort.timestamp, count: 1, direction: SearchSortDirection.asc);

            if (searchResults != null && searchResults.messages.matches.Any())
            {
                _logger.LogInformation(JsonConvert.SerializeObject(searchResults.messages.matches));
                var r = searchResults.messages.matches.FirstOrDefault();

                if (r == null)
                {
                    return LogHandled($"NO-URL-IN-MSG");
                }

                if (r.user == incomingMessage.User.Id)
                {
                    return LogHandled("OLD-BUT-SAME-USER-SO-IGNORING");

                }

                var threadTs = incomingMessage.Timestamp.ToString("N6");
                var reactionResponses = await AddReactions(incomingMessage.ChatHub.Id, threadTs);

                var message = $"postet av @{r.username} for {TimeSpanExtensions.Ago(r.ts)} siden.";

                var chatMessage = new ChatMessage
                {
                    Channel = incomingMessage.ChatHub.Id,
                    Parse = "full",
                    Link_Names = 1,
                    thread_ts = threadTs,
                    unfurl_links = "false",
                    unfurl_media = "true",
                    as_user = "false",
                    Text = r.permalink,
                    attachments = new[]
                    {
                        new Attachment {text = $":older_man: {message}", color = "#FF0000"}
                    }
                };

                var response = await _slackClient.PostMessageAsync(incomingMessage.ChatHub.Id,
                    r.permalink,
                    parse: "full",
                    attachments: new []
                    {
                        new Attachment { text = $":older_man: {message}", color = "#FF0000" }
                    }, thread_ts: threadTs);


                _logger.LogInformation("Sent message. Response:" + JsonConvert.SerializeObject(response));

                var reactionResponseBody = reactionResponses.First();
                _logger.LogInformation("Sent reaction. Response:" + JsonConvert.SerializeObject(reactionResponseBody));


                return LogHandled("OLD");
            }
            return LogHandled("NEW");
        }

        private Task<ReactionAddedResponse[]> AddReactions(string channelId, string thread_ts)
        {
            var t1 = _slackClient.AddReactionAsync("older_man", channelId, thread_ts);
            var t2 = _slackClient.AddReactionAsync("older_man::skin-tone-2",channelId, thread_ts);
            var t3 = _slackClient.AddReactionAsync("older_man::skin-tone-3",channelId, thread_ts);
            var t4 = _slackClient.AddReactionAsync("older_man::skin-tone-4",channelId, thread_ts);
            var t5 = _slackClient.AddReactionAsync("older_man::skin-tone-5",channelId, thread_ts);
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