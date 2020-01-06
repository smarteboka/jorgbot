using Shouldly;
using SlackConnector.Connections.Sockets.Messages.Inbound;
using SlackConnector.Extensions;
using SlackConnector.Models;
using Xunit;

namespace SlackConnector.Tests.Unit.Extensions
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