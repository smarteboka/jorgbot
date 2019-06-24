using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oldbot.Utilities;
using Oldbot.Utilities.SlackAPI.Extensions;
using SlackConnector.Models;
using SearchSort = Oldbot.Utilities.SlackAPIFork.SearchSort;
using SearchSortDirection = Oldbot.Utilities.SlackAPIFork.SearchSortDirection;


namespace Oldbot.ConsoleApp
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

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of blogs</returns>
        public async Task<string> Validate(SlackMessage incomingMessage)
        {
            _logger.LogInformation("Received:" + incomingMessage.RawData);


            if (incomingMessage.User.IsBot)
            {
                return await Respond("BOT", incomingMessage);
            }
            
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
                        return await Respond($"NO-URL-IN-MSG", incomingMessage);
                  
                    if (r.user == incomingMessage.User.Name)
                        return await Respond("OLD-BUT-SAME-USER-SO-IGNORING", incomingMessage);
                    
                    _logger.LogInformation($"inc-ts:{incomingMessage.Timestamp.ToString("N6")}");
                    var threadTs = incomingMessage.Timestamp.ToString("N6");
                    
                    var reactionResponse = await _slackClient.AddReactions(incomingMessage.ChatHub.Id, threadTs);
                 
                    var message = $"postet av @{r.username} for {TimeSpanExtensions.Ago(r.ts)} siden.";
                    var response = await _slackClient.SendMessage(incomingMessage.ChatHub.Id, message, threadTs, r.permalink);
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Sent message. Response:" + JsonConvert.SerializeObject(body));

                    var reactionResponseBody = await reactionResponse.First().Content.ReadAsStringAsync();
                    _logger.LogInformation("Sent reaction. Response:" + JsonConvert.SerializeObject(reactionResponseBody));


                    return await Respond($"OLD", incomingMessage);
                    
                }
                return await Respond($"NEW", incomingMessage);
            }

            return await Respond($"NO-URL-IN-MSG", incomingMessage);
        }

        private Task<string> Respond(string body, SlackMessage context)
        {
            _logger.LogInformation($"Treated as: {body}");
            return Task.FromResult(body);
        }
    }
}
