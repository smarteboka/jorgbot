using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackAPI;
using Slackbot.Net.Strategies;
using Slackbot.Net.Utilities;
using Slackbot.Net.Utilities.SlackAPI.Extensions;
using SlackConnector.Models;
using SearchSort = Slackbot.Net.Utilities.SlackAPIFork.SearchSort;
using SearchSortDirection = Slackbot.Net.Utilities.SlackAPIFork.SearchSortDirection;

namespace Smartbot.Utilities.Handlers
{
    public class OldHandler : IHandleMessages
    {
        private readonly ILogger<OldHandler> _logger;
        private readonly ISlackClient _slackClient;

        public OldHandler(ILogger<OldHandler> logger, ISlackClient slackClient)
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
                var reactionResponse = await _slackClient.AddReactions(incomingMessage.ChatHub.Id, threadTs);

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

                var response = await _slackClient.SendMessage(chatMessage);

                var body = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Sent message. Response:" + JsonConvert.SerializeObject(body));

                var reactionResponseBody = await reactionResponse.First().Content.ReadAsStringAsync();
                _logger.LogInformation("Sent reaction. Response:" + JsonConvert.SerializeObject(reactionResponseBody));


                return LogHandled("OLD");
            }
            return LogHandled("NEW");
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