using Smartbot.Utilities.FourSquareServices.Entities;

namespace Smartbot.Utilities.FourSquareServices.Core
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