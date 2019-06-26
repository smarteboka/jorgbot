using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FourSquare.SharpSquare.Entities;
using Microsoft.Extensions.Logging;
using Oldbot.Utilities.FourSquareServices;
using SlackConnector.Models;
using Smartbot.Publishers;

namespace Smartbot.HostedServices.Strategies
{
    public class FoursquareStrategy : IReplyStrategy
    {
        private readonly FourSquareService _foursquare;
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly ILogger<FoursquareStrategy> _logger;

        private List<string> Categories = new List<string>
        {
            "food", "drink", 
            "coffee", "shops", 
            "arts", "outdoors", 
            "sights", "trending", 
            "topPicks" 
        };

        public FoursquareStrategy(FourSquareService foursquare, IEnumerable<IPublisher> publishers, ILogger<FoursquareStrategy> logger)
        {
            _foursquare = foursquare;
            _publishers = publishers;
            _logger = logger;
        }

        public async Task Handle(SlackMessage message)
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
                    Channel = message.ChatHub.Id
                };
                await publisher.Publish(notification);
            }
        }

        public bool ShouldExecute(SlackMessage message)
        {
            var execute = message.Text.StartsWith("<@UGWC87WRZ> 4sq", StringComparison.InvariantCultureIgnoreCase);
            return execute;
        }
    }
}