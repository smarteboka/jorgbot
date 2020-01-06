using System;

namespace Slackbot.Net.SlackClients.Rtm.Logging
{
    public class Logger : ILogger
    {
        public void LogError(string message)
        {
            Console.WriteLine(message);
        }
    }
}