using System;
using System.Linq;
using System.Threading.Tasks;
using Oldbot.Utilities.FourSquareServices;
using Xunit;

namespace Oldbot.OldFunction.Tests
{
    public class FourSquareTests
    {
        [Fact (Skip = "exploratory test")]        
        public void Test()
        {
            var fs = CreateFourSquareService();
            var venues = fs.GetOsloVenuesByQuery("beer");
            Assert.NotEmpty(venues);
            
            Assert.Equal(0, venues.First().venue.rating);
            var firstVenue = venues.Select(v => v.venue.name).Aggregate((x,y) => x + "," + y);
            Assert.Equal("stuff", firstVenue);
        }

        private static FourSquareService CreateFourSquareService()
        {
            var clientId = Environment.GetEnvironmentVariable("Smartbot_Foursquare_ClientId");
            var clientSecret = Environment.GetEnvironmentVariable("Smartbot_Foursquare_ClientSecret");
            return new FourSquareService(clientId, clientSecret);
        }
    }
}