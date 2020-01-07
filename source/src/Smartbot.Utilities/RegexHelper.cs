using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Smartbot.Utilities
{
    public static class RegexHelper
    {
        private static readonly Regex UrlsRegex = new Regex("((http(s)?://|www\\.)([A-Z0-9.\\-:]{1,})\\.[0-9A-Z?;*:~&#=\\-_\\./]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static MatchCollection FindURl(string messageText)
        {
            return UrlsRegex.Matches(messageText);
        }

        public static IEnumerable<string> FindUrls(string messageText)
        {
            IEnumerable<Match> matches = UrlsRegex.Matches(messageText).Cast<Match>();
            return matches.Select(m => m.Value);
        }

        public static readonly Regex ChannelsRegex = new Regex("<#\\w+\\|(\\w+)>", RegexOptions.Compiled);

        public static string FindChannelName(string messageText)
        {
            var matches = ChannelsRegex.Matches(messageText).Cast<Match>();
            if (matches.Any() && matches.First().Groups.Cast<Group>().Any())
            {
                return matches.First().Groups.Cast<Group>().Last().Value;
            }

            return null;
        }

        public static string RemoveChannel(string messageText)
        {
            return ChannelsRegex.Replace(messageText, "",10);
        }

        public static readonly Regex UserRegex = new Regex("<@\\w+>", RegexOptions.Compiled);


        public static string RemoveUser(string messageText)
        {
            return UserRegex.Replace(messageText, "");
        }
    }
}