using System;
using Smartbot.Utilities.Handlers._4sq.FourSquareServices;
using Xunit;

namespace Smartbot.Tests.Workers
{
    public class FourSquareTests
    {
        [Fact]
        public void CanFetchVenues()
        {
            var fs = CreateFourSquareService();
            var venues = fs.GetOsloVenuesByQuery("øl");
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