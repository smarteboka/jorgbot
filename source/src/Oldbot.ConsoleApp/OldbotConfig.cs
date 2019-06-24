using System;

namespace Oldbot.ConsoleApp
{
    internal class OldbotConfig
    {
        public string SlackApiKeyBotUser { get; } = Environment.GetEnvironmentVariable("OldBot_SlackApiKey_BotUser");
        
        public string SlackApiKeySlackApp { get; } = Environment.GetEnvironmentVariable("OldBot_SlackApiKey_SlackApp");
    }
}