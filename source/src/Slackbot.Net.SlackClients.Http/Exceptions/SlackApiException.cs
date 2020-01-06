using System;

namespace Slackbot.Net.SlackClients.Exceptions
{
    public class SlackApiException : Exception
    {
        public SlackApiException(string s) : base(s)
        {
            
        }
    }
}