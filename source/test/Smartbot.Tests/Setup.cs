using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Slackbot.Net.SlackClients;
using Slackbot.Net.SlackClients.Http;
using Slackbot.Net.SlackClients.Http.Extensions;
using Xunit.Abstractions;

namespace Smartbot.Tests
{
    public class Setup
    {
        public readonly ISearchClient SearchClient;
        public readonly ISlackClient SlackClient;
        protected string Channel;
        protected string Text;

        public Setup(ITestOutputHelper helper)
        {
            var services = new ServiceCollection();
            services.AddSlackbotOauthClient(c =>
            {
                c.OauthToken = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_SlackApp");
            });
            
            services.AddSlackbotClient(c =>
            {
                c.BotToken = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_BotUser");
            });

            services.AddSingleton<ILogger<ISlackClient>>(new XUnitLogger<ISlackClient>(helper));
            
            var provider = services.BuildServiceProvider();
            SearchClient = provider.GetService<ISearchClient>();
            SlackClient = provider.GetService<ISlackClient>();
            Channel = "#testss";
            Text = "Test";
        }
    }
}