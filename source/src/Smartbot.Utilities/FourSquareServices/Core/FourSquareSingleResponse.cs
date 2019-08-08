using System.Collections.Generic;
using Smartbot.Utilities.FourSquareServices.Entities;

namespace Smartbot.Utilities.FourSquareServices.Core
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



