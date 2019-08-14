using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Handlers;
using Slackbot.Net.Publishers;
using SlackConnector.Models;
using Smartbot.Utilities.FourSquareServices;
using Smartbot.Utilities.FourSquareServices.Entities;

namespace Smartbot.Utilities.Handlers
{
    public class FourSquareHandler : IHandleMessages
    {
        private readonly FourSquareService _foursquare;
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly ILogger<FourSquareHandler> _logger;

        private List<string> Categories = new List<string>
        {
            "food", "drink",
            "coffee", "shops",
            "arts", "outdoors",
            "sights", "trending",
            "topPicks"
        };

        public FourSquareHandler(FourSquareService foursquare, IEnumerable<IPublisher> publishers, ILogger<FourSquareHandler> logger)
        {
            _foursquare = foursquare;
            _publishers = publishers;
            _logger = logger;
        }

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var matchingCategory = Categories.FirstOrDefault(c => message.Text.Contains(c, StringComparison.InvariantCultureIgnoreCase));
            var venueExplores = new List<VenueExplore>();
            if (matchingCategory != null)
            {
                _logger.LogInformation($"Searching by category {matchingCategory}");
                var osloVenuesByCategory = _foursquare.GetOsloVenuesByCategory(matchingCategory);
                venueExplores = new List<VenueExplore>(osloVenuesByCategory);
            }
            else
            {
                var messageText = message.Text.Replace("<@UGWC87WRZ> 4sq ", "");
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

            foreach (var publisher in _publishers)
            {
                var notification = new Notification
                {
                    Msg = sb.ToString(),
                    Recipient = message.ChatHub.Id
                };
                await publisher.Publish(notification);
            }
            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            var execute = message.Text.StartsWith("<@UGWC87WRZ> 4sq", StringComparison.InvariantCultureIgnoreCase);
            return execute;
        }
    }
}