namespace Slackbot.Net.SlackClients.Rtm.BotHelpers
{
    internal interface IMentionDetector
    {
        bool WasBotMentioned(string username, string userId, string messageText);
    }
}