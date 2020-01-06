using System.Linq;
using Slackbot.Net.SlackClients.Rtm.Connections.Models;
using Slackbot.Net.SlackClients.Rtm.Models;

namespace Slackbot.Net.SlackClients.Rtm.Extensions
{
    internal static class ImExtensions
    {
        public static SlackChatHub ToChatHub(this Im im, SlackUser[] users)
        {
            SlackUser user = users.FirstOrDefault(x => x.Id == im.User);
            return new SlackChatHub
            {
                Id = im.Id,
                Name = "@" + (user == null ? im.User : user.Name),
                Type = SlackChatHubType.DM
            };
        }
    }
}
