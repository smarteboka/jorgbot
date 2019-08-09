using System;
using System.Collections.Generic;

namespace Smartbot.Utilities.FourSquareServices.Entities
{
    public class FourSquareEntityGroups<T> : FourSquareEntity where T : FourSquareEntity
    {
        public Int64 count
        {
            get;
            set;
        }

        public string summary
        {
            get;
            set;
        }

        public List<FourSquareEntityItems<T>> groups
        {
            get;
            set;
        }
    }
}