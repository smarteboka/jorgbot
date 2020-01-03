using System;
using Slackbot.Net.Abstractions.Handlers;
using SlackConnector.Models;
using SlackMessage = Slackbot.Net.Abstractions.Handlers.SlackMessage;

namespace Slackbot.Net.Connections
{
    public class SlackConnectorMapper
    {
        public static SlackMessage Map(SlackConnector.Models.SlackMessage msg)
        {
            var slackMessage = new SlackMessage
            {
                Text = msg.Text,
                RawData = msg.RawData,
                Timestamp = msg.Timestamp,
                User = ToUser(msg.User),
                MentionsBot = msg.MentionsBot,
                ChatHub = new ChatHub
                {
                    Id = msg.ChatHub?.Id,
                    Name = msg.ChatHub?.Name,
                    Type = EnumToString(msg.ChatHub)
                }
            };
            return slackMessage;
        }

        private static User ToUser(SlackUser messageUser)
        {
            return new User
            {
                Id = messageUser.Id,
                Email = messageUser.Email,
                FirstName = messageUser.FirstName,
                LastName = messageUser.LastName,
                Image = messageUser.Image,
                Name = messageUser.Name,
                IsBot = messageUser.IsBot
            };

        }

        private static string EnumToString(SlackChatHub chatHubType)
        {
            if (chatHubType == null)
                return "Unknown";
            
            switch (chatHubType.Type)
            {
                case SlackChatHubType.DM:
                    return ChatHubTypes.DirectMessage;
                case SlackChatHubType.Channel:
                    return ChatHubTypes.Channel;
                case SlackChatHubType.Group:
                    return ChatHubTypes.Group;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}