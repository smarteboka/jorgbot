using System.Collections.Generic;
using Smartbot.Utilities.Handlers._4sq.FourSquareServices.Entities;

namespace Smartbot.Utilities.Handlers._4sq.FourSquareServices.Core
{
    public class FourSquareSingleResponse<T> : FourSquareResponse where T : FourSquareEntity
    {
        public Dictionary<string, T> response
        {
            get;
            set;
        }
    }
}



