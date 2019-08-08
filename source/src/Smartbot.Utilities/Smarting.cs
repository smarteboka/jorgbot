using System;

namespace Smartbot.Utilities
{
    public class Smarting
    {
        public Smarting(string name, int year, int month, int day)
        {
            Name = name;
            BirthDate = new DateTime(year, month, day);
        }

        public DateTime BirthDate
        {
            get;
        }

        public string Name
        {
            get;
        }
    }
}