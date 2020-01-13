using System.Collections.Generic;

namespace Smartbot.Utilities.Handlers._4sq.FourSquareServices.Entities
{
    public class Hour : FourSquareEntity
    {
        public string status
        {
            get;
            set;
        }

        public bool isOpen
        {
            get;
            set;
        }

        public List<TimeFrame> timeframes
        {
            get;
            set;
        }

        public List<Segment> segments
        {
            get;
            set;
        }
    }
}