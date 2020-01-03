using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net.Configuration;
using Slackbot.Net.Handlers;
using SlackConnector;

namespace Slackbot.Net.Connections
{
    internal class SlackConnectionSetup
    {
        private readonly IServiceProvider _services;

        public SlackConnectionSetup(IServiceProvider services)
        {
            _services = services;
        }

        public async Task Connect()
        {
            var options = _services.GetService<IOptions<SlackOptions>>();
            var logger = _services.GetService<ILogger<SlackConnector.SlackConnector>>();
            var slackConnector = new SlackConnector.SlackConnector();
            var handlerSelector = _services.GetService<HandlerSelector>();
            Connection = await slackConnector.Connect(options.Value.Slackbot_SlackApiKey_BotUser);
            Connection.OnMessageReceived += msg => handlerSelector.HandleIncomingMessage(SlackConnectorMapper.Map(msg));
            
            if (Connection.IsConnected)
                logger.LogInformation("Connected");

        }

        public ISlackConnection Connection { get; private set; }
    }
}