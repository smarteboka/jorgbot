﻿using System.Collections.Generic;

namespace Smartbot.Utilities.FourSquareServices.Entities
{
    public class Image : FourSquareEntity
    {
        public string prefix
        {
            get;
            set;
        }

        public List<string> sizes
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }
    }
}