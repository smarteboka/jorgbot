using System.Text.RegularExpressions;

namespace Slackbot.Net.SlackClients.Rtm.BotHelpers
{
    internal class MentionDetector : IMentionDetector
    {
        public bool WasBotMentioned(string username, string userId, string messageText)
        {
            bool mentioned = false;

            if (!string.IsNullOrEmpty(messageText))
            {
                string regexText = $"<@{userId}>|{username}";
                mentioned = Regex.IsMatch(messageText, regexText, RegexOptions.IgnoreCase);
            }

            return mentioned;
        }
    }
}