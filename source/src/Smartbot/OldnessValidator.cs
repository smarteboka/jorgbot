using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oldbot.Utilities;
using Oldbot.Utilities.SlackAPI.Extensions;
using Oldbot.Utilities.SlackAPIFork;
using SlackConnector.Models;

namespace Smartbot
{
    public class OldnessValidator
    {
      
        private readonly ISlackClient _slackClient;
        private readonly ILogger<OldnessValidator> _logger;

        public OldnessValidator(ISlackClient slackClient, ILogger<OldnessValidator> logger)
        {
            _slackClient = slackClient;
            _logger = logger;
        }
  
        public async Task<string> Validate(SlackMessage incomingMessage)
        {
            _logger.LogInformation("Received:" + incomingMessage.RawData);


            if (incomingMessage.User.IsBot)
            {
                return await LogHandled("BOT", incomingMessage);
            }

            if (string.IsNullOrEmpty(incomingMessage.Text))
                return await LogHandled("IGNORED", incomingMessage);
            
            var urls = RegexHelper.FindURl(incomingMessage.Text);

            if (urls.Any())
            {
                var firstUrl = urls.First();
                var cleansedUrl = UrlCleaner.CleanForTrackingQueryParams(firstUrl.Value);
                cleansedUrl = cleansedUrl.TrimEnd('/');
                var searchResults = await _slackClient.SearchMessagesAsync(cleansedUrl, SearchSort.timestamp, count: 1, direction: SearchSortDirection.asc);

                if (searchResults != null && searchResults.messages.matches.Any())
                {
                    _logger.LogInformation(JsonConvert.SerializeObject(searchResults.messages.matches));
                    var r = searchResults.messages.matches.FirstOrDefault();

                    if (r == null)
                        return await LogHandled($"NO-URL-IN-MSG", incomingMessage);
                    Console.WriteLine($"{r.user} vs {incomingMessage.User.Id}");
                    if (r.user == incomingMessage.User.Id)
                        return await LogHandled("OLD-BUT-SAME-USER-SO-IGNORING", incomingMessage);
                    
                    var threadTs = incomingMessage.Timestamp.ToString("N6");
                    var reactionResponse = await _slackClient.AddReactions(incomingMessage.ChatHub.Id, threadTs);
                 
                    var message = $"postet av @{r.username} for {TimeSpanExtensions.Ago(r.ts)} siden.";
                    var response = await _slackClient.SendMessage(incomingMessage.ChatHub.Id, message, threadTs, r.permalink);
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Sent message. Response:" + JsonConvert.SerializeObject(body));

                    var reactionResponseBody = await reactionResponse.First().Content.ReadAsStringAsync();
                    _logger.LogInformation("Sent reaction. Response:" + JsonConvert.SerializeObject(reactionResponseBody));


                    return await LogHandled($"OLD", incomingMessage);
                    
                }
                return await LogHandled($"NEW", incomingMessage);
            }

            return await LogHandled($"NO-URL-IN-MSG", incomingMessage);
        }

        private Task<string> LogHandled(string body, SlackMessage context)
        {
            _logger.LogInformation($"Treated as: {body}");
            return Task.FromResult(body);
        }
    }
}