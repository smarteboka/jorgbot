using System.Collections.Generic;
using Smartbot.Utilities.Handlers._4sq.FourSquareServices.Entities;

namespace Smartbot.Utilities.Handlers._4sq.FourSquareServices.Core
{
    public class FourSquareMultipleResponse<T> : FourSquareResponse where T : FourSquareEntity
    {
        public Dictionary<string, List<T>> response
        {
            get;
            set;
        }
    }
}