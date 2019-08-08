using System;
using System.Linq;
using System.Threading.Tasks;
using Smartbot.Utilities.FourSquareServices;
using Xunit;

namespace Oldbot.OldFunction.Tests
{
    public class FourSquareTests
    {
        [Fact]        
        public void CanFetchVenues()
        {
            var fs = CreateFourSquareService();
            var venues = fs.GetOsloVenuesByQuery("beer");
            Assert.NotEmpty(venues);
        }

        private static FourSquareService CreateFourSquareService()
        {
            var clientId = Environment.GetEnvironmentVariable("Smartbot_Foursquare_ClientId");
            var clientSecret = Environment.GetEnvironmentVariable("Smartbot_Foursquare_ClientSecret");
            return new FourSquareService(clientId, clientSecret);
        }
    }
}