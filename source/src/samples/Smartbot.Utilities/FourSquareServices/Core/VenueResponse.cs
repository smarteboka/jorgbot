using System.Collections.Generic;

namespace Smartbot.Utilities.FourSquareServices.Core
{
    public class VenueResponse<T>
    {
        public Dictionary<string, object> geocoded
        {
            get;
            set;
        }
        public List<T> venues
        {
            get;
            set;
        }
    }
}