using Shouldly;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound;
using Slackbot.Net.SlackClients.Rtm.Extensions;
using Slackbot.Net.SlackClients.Rtm.Models;
using Xunit;

namespace Slackbot.Net.SlackClients.Rtm.Tests.Unit.Extensions
{
    public class MessageSubTypeExtensionsTests
    {
        [Theory]
        [InlineData(MessageSubType.bot_message, SlackMessageSubType.BotMessage)]
        private void should_convert_to_expected_enum(MessageSubType inbound, SlackMessageSubType expected)
        {
            inbound
                .ToSlackMessageSubType()
                .ShouldBe(expected);
        }
    }
}