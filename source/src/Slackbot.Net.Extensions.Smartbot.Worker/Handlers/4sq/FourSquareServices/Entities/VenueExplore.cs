namespace Smartbot.Utilities.Handlers._4sq.FourSquareServices.Entities
{
    public class VenueExplore : FourSquareEntity
    {
        public FourSquareEntityItems<Reasons> reasons
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