namespace Smartbot.Utilities.Handlers._4sq.FourSquareServices.Entities
{
    public class VenueHistory : FourSquareEntity
    {
        public string beenHere
        {
            get;
            set;
        }

        public string lastHereAt
        {
            get;
            set;
        }

        public Venue venue
        {
            get;
            set;
        }
	}
}