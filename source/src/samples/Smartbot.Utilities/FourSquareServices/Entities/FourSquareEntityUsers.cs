using System;

namespace Smartbot.Utilities.FourSquareServices.Entities
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
