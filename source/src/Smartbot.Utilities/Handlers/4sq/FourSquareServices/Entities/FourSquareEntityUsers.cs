using System;

namespace Smartbot.Utilities.Handlers._4sq.FourSquareServices.Entities
{
    public class FourSquareEntityUsers : FourSquareEntity
    {
        public Int64 count
        {
            get;
            set;
        }

        public User user
        {
            get;
            set;
        }
    }
}
