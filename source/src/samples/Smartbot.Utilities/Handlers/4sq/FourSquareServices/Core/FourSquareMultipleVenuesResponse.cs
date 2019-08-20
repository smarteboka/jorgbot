using Smartbot.Utilities.Handlers._4sq.FourSquareServices.Entities;

namespace Smartbot.Utilities.Handlers._4sq.FourSquareServices.Core
{
    public class FourSquareMultipleVenuesResponse<T> : FourSquareResponse where T : FourSquareEntity
    {
        public VenueResponse<T> response
        {
            get;
            set;
        }
    }
}