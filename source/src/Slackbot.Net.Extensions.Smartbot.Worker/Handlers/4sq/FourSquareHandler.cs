using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Endpoints.Abstractions;
using Slackbot.Net.Endpoints.Models.Events;
using Slackbot.Net.SlackClients.Http;
using Smartbot.Utilities.Handlers._4sq.FourSquareServices;
using Smartbot.Utilities.Handlers._4sq.FourSquareServices.Entities;

namespace Smartbot.Utilities.Handlers._4sq
{
    public class FourSquareHandler : IHandleAppMentions
    {
        private readonly FourSquareService _foursquare;
        private readonly ISlackClient _client;
        private readonly ILogger<FourSquareHandler> _logger;

        private List<string> Categories = new List<string>
        {
            "food", "drink",
            "coffee", "shops",
            "arts", "outdoors",
            "sights", "trending",
            "topPicks"
        };

        public FourSquareHandler(FourSquareService foursquare, ISlackClient client, ILogger<FourSquareHandler> logger)
        {
            _foursquare = foursquare;
            _client = client;
            _logger = logger;
        }

        public (string HandlerTrigger, string Description) GetHelpDescription() => ("4sq <search>", "Søker i FourSquare etter <search> i Oslo");

        public async Task<EventHandledResponse> Handle(EventMetaData eventMetadata, AppMentionEvent slackEvent)
        {
            var matchingCategory = Categories.FirstOrDefault(c => slackEvent.Text.Contains(c, StringComparison.InvariantCultureIgnoreCase));
            var venueExplores = new List<VenueExplore>();
            if (matchingCategory != null)
            {
                _logger.LogInformation($"Searching by category {matchingCategory}");
                var osloVenuesByCategory = _foursquare.GetOsloVenuesByCategory(matchingCategory);
                venueExplores = new List<VenueExplore>(osloVenuesByCategory);
            }
            else
            {
               
                var messageText = slackEvent.Text.Replace($"4sq ", "");
                _logger.LogInformation($"Searching by query {messageText}");
                var osloVenuesByQuery = _foursquare.GetOsloVenuesByQuery(messageText);
                venueExplores = new List<VenueExplore>(osloVenuesByQuery);
            }

            var sb = new StringBuilder();
            foreach (var venueExplore in venueExplores)
            {
                var venue = venueExplore.venue;
                sb.Append($"{venue.name} https://foursquare.com/v/smarteboka/{venue.id} \n");
            }

            if (!venueExplores.Any())
                sb.Append("Fant nada :/ Prøv å søke etter noe annet a");

            
            await _client.ChatPostMessage(slackEvent.Channel, sb.ToString());
            
            return new EventHandledResponse("OK");
        }


        public bool ShouldHandle(AppMentionEvent message) => message.Text.StartsWith($"4sq", StringComparison.InvariantCultureIgnoreCase);
           
        
    }
}