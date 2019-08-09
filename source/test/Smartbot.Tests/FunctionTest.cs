using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Integrations.SlackAPIFork;
using Slackbot.Net.Utilities;
using SlackConnector.Models;
using Smartbot.Utilities.Handlers;
using Xunit;

namespace Smartbot.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task EmptyBodyWorks()
        {
            var request = new SlackMessage
            {
                Text = null,
                User = new SlackUser()
            };

            var validateOldness = new OldHandler(new NoopLogger(),new MockClient());
            var response = await validateOldness.Handle(request);
            Assert.Equal("IGNORED", response.HandledMessage);
        }


        [Fact]
        public async Task FindingUrlsWorks()
        {
            await TestIt("OLD", "la oss se litt på https://www.juggel.no", "det var noe https://www.juggel.no greier da");
            await TestIt("OLD", "<https://ilaks.no/na-kan-du-kjope-norsk-laks-pa-automater-i-singapore/>", "keen på https://ilaks.no/na-kan-du-kjope-norsk-laks-pa-automater-i-singapore");
            await TestIt("OLD", "https://ilaks.no/na-kan-du-kjope-norsk-laks-pa-automater-i-singapore", "alle vil ha laks https://ilaks.no/na-kan-du-kjope-norsk-laks-pa-automater-i-singapore");
            await TestIt("OLD", "GI meg gi meg lox <https://ilaks.no/na-kan-du-kjope-norsk-laks-pa-automater-i-singapore/>", "https://ilaks.no/na-kan-du-kjope-norsk-laks-pa-automater-i-singapore");
        }

        [Fact]
        public async Task NoText()
        {
            await TestIt("IGNORED", null, "");
            await TestIt("IGNORED", "", "");
        }

        [Fact]
        public void SkipsBotMessages()
        {
            var request = new SlackMessage
            {
                Text = "OLD! https://www.aftenposten.no/norge/i/L08awV/Haper-pa-mer-enn-ti-tusen-barn-og-unge-i-norske-klimastreiker?utm_source=my-unit-test",
                User = new SlackUser
                {
                    IsBot = true
                }
            };

            var validateOldness = new OldHandler(new NoopLogger(),new MockClient());

            Assert.False(validateOldness.ShouldHandle(request));
        }

        [Fact]
        public async Task DoesNotOldIfIsSameAuthor()
        {
            var mockClient = new MockClient();

            var existingMessage = new ContextMessage
            {
                text = "A historic tale. I told you about http://db.no some time ago",
                user = "U0F3P72QM",
                ts = "1550000000.000000" //
            };
            mockClient.SetSearchResponse(existingMessage);

            var request = new SlackMessage
            {
                Text = "Woot, me, U0F3P72QM, is repeating the url http://db.no some time later",
                User = new SlackUser
                {
                    Id = "U0F3P72QM"
                },
                Timestamp = 1660000000.000000,
                ChatHub = new SlackChatHub
                {
                    Id = "123"
                }
            };

            var validateOldness = new OldHandler(new NoopLogger(),mockClient);

            var response = await validateOldness.Handle(request);
            Assert.Equal("OLD-BUT-SAME-USER-SO-IGNORING", response.HandledMessage);
        }

        [Fact]
        public void TestFinders()
        {
            AssertChannelRegex("tests", "say this is a thing <#CGY1XJRM1|tests> with other things");
        }

        [Fact]
        public void TestUrlFinder()
        {
            var expected = "https://itunes.apple.com/no/podcast/272-build-tech-anett-andreassen-digitalisering-i-byggebransjen/id1434899825?i=1000431627999&amp;l=nb&amp;mt=2";
            var message =  "https://itunes.apple.com/no/podcast/272-build-tech-anett-andreassen-digitalisering-i-byggebransjen/id1434899825?i=1000431627999&amp;l=nb&amp;mt=2";
            AssertUrlRegex(expected, message);
            AssertUrlRegex("https://edition-m.cnn.com/2019/03/24/politics/mueller-report-release/index.html", "<https://edition-m.cnn.com/2019/03/24/politics/mueller-report-release/index.html>");
            AssertUrlRegex("https://www.linkedin.com/feed/update/urn:li:activity:6545200308086284288", "https://www.linkedin.com/feed/update/urn:li:activity:6545200308086284288");
            AssertUrlRegex("https://www.nrk.no/video/PS*ce0b6a6b-5a06-4135-a0cd-a56f88440b65", "https://www.nrk.no/video/PS*ce0b6a6b-5a06-4135-a0cd-a56f88440b65");
        }

        private static void AssertChannelRegex(string expected, string input)
        {
            Assert.Equal(expected, RegexHelper.FindChannelName(input));
        }

        private static void AssertUrlRegex(string expected, string input)
        {
            var matches = RegexHelper.FindUrls(input);
            var match = matches.Any() ? matches.First() : null;

            Assert.Equal(expected, match);
        }

        private static async Task TestIt(string expected, string slackMessage, string historicMessage)
        {
            var mock = new MockClient();
            var newMessage = new ContextMessage {
                //channel = "CGWGZ90KV", // private channel #bottestsmore
                text = historicMessage,
                ts = "1552671370.000200", //older
                user = "another-smarting"
            };
            mock.SetSearchResponse(newMessage);
            var validateOldness = new OldHandler(new NoopLogger(),mock);

            var response = await validateOldness.Handle(new SlackMessage
            {
                Text = slackMessage,
                User = new SlackUser
                {

                },
                ChatHub = new SlackChatHub
                {
                    Id = "123"
                }
            });
            Assert.Equal(expected, response.HandledMessage);
        }
    }

    public class NoopLogger : ILogger<OldHandler>
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
