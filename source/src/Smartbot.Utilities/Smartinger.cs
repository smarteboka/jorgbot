using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Slackbot.Net.Utilities;

namespace Smartbot.Utilities
{
    public class Smartinger
    {
        private readonly Timing _timing;

        public List<Smarting> Smartingene = new List<Smarting>
        {
            new Smarting("@mrmoen", 1982, 1, 24),
            new Smarting("@nash", 1982, 1, 27),
            new Smarting("@ef", 1982, 3, 16),
            new Smarting("@thodd", 1982, 3, 18),
            new Smarting("@glaub", 1982, 6, 7),
            new Smarting("@tomasutnehh", 1980, 6, 23),
            new Smarting("@trondod", 1982, 7, 5),
            new Smarting("@tigertor", 1982, 9, 13),
            new Smarting("@kristmel", 1982, 9, 13),
            new Smarting("@jmrandby", 1982, 10, 17),
            new Smarting("@jarlelin", 1982, 10, 19),
            new Smarting("@john", 1982, 10, 21),
            new Smarting("@mariustu", 1982, 12, 3),
            new Smarting("@fsivertsen", 1981, 3, 21)
        };

        public Smartinger()
        {
            _timing = new Timing();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _timing.SetTimeZone("Europe/Oslo");
            }
            else
            {
                _timing.SetTimeZone("Central European Standard Time");
            }
        }

        public IEnumerable<Smarting> ThatHasBirthday()
        {
            return Smartingene.Where(s => _timing.IsToday(s.BirthDate)).Select(s => s);
        }
    }
}