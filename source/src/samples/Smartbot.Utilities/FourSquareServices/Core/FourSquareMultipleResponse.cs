using System.Collections.Generic;
using Smartbot.Utilities.FourSquareServices.Entities;

namespace Smartbot.Utilities.FourSquareServices.Core
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