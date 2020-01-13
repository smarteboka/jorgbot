using System;
using System.Collections.Generic;

namespace Smartbot.Utilities.Handlers._4sq.FourSquareServices.Entities
{
    public class FourSquareEntityItems<T> : FourSquareEntity where T : FourSquareEntity
    {
        public string type
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public Int64 count
        {
            get;
            set;
        }

        public List<T> items
        {
            get;
            set;
        }
    }
}