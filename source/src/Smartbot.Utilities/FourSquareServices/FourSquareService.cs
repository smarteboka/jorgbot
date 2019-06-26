using System.Collections;
using System.Collections.Generic;
using FourSquare.SharpSquare.Core;
using FourSquare.SharpSquare.Entities;
using Microsoft.Extensions.Options;

namespace Oldbot.Utilities.FourSquareServices
{
    public class FourSquareService : IFoursquareService
    {

        private readonly SharpSquare _sharpSquare;

        public FourSquareService(IOptions<FourSquareOptions> options)
        {
            _sharpSquare = new SharpSquare(options.Value.Smartbot_Foursquare_ClientId, options.Value.Smartbot_Foursquare_ClientSecret);
        }
        
        public FourSquareService(string clientId, string clientSecret)
        {
            _sharpSquare = new SharpSquare(clientId, clientSecret);
        }

        public IEnumerable<VenueExplore> GetOsloVenuesByQuery(string query)
        {
            return GetOsloVenues("query", query);
        }
        
        public IEnumerable<VenueExplore> GetOsloVenuesByCategory(string section)
        {
            return GetOsloVenues("section", section);
        }

        public IEnumerable<VenueExplore> GetOsloVenues(string key, string value)
        {
            var parameters = new Dictionary<string, string>
            {
                { "ll","59.914491,10.74933" },
                { "limit", "20" },
                { "price", "2,3,4,5"},
                { "radius", "5000"},
                { key, value}
            };
         
            return _sharpSquare.ExploreVenues(parameters);
        }
    }

    public interface IFoursquareService
    {
    }
}