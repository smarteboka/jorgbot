using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net.Handlers;
using Slackbot.Net.Publishers.Slack;
using SlackConnector;
using SlackConnector.Models;

namespace Slackbot.Net
{
    internal class SlackConnectorHostedService : BackgroundService
    {
        private readonly ISlackConnector _noobotCore;
        private readonly ILogger<SlackConnectorHostedService> _logger;
        private readonly HandlerSelector _handlerSelector;
        private readonly SlackOptions _config;
        private ISlackConnection _connection;
        private bool _connected;

        public SlackConnectorHostedService(ISlackConnector noobotCore, IOptions<SlackOptions> options, ILogger<SlackConnectorHostedService> logger, HandlerSelector handlerSelector)
        {
            _noobotCore = noobotCore;
            _logger = logger;
            _handlerSelector = handlerSelector;
            _config = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && !_connected)
            {
                _logger.LogInformation("Connecting");
                _connection = await _noobotCore.Connect(_config.Slackbot_SlackApiKey_BotUser);
                _connection.OnMessageReceived += HandleIncomingMessage;

                _connection.OnDisconnect += () =>
                {
                    _connected = false;
                    _logger.LogInformation("Disconnecting");
                };
                if (_connection.IsConnected)
                {
                    _logger.LogInformation("Connected");
                    _connected = true;
                }
            }
        }

        private async Task HandleIncomingMessage(SlackMessage message)
        {
            var handlers = _handlerSelector.SelectHandler(message);
            foreach (var handler in handlers)
            {
                try
                {
                    await handler.Handle(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
        }

        public override Task StopAsync(CancellationToken token)
        {
            _logger.LogInformation("Closing");
             _connection.Close().GetAwaiter().GetResult();
            _logger.LogInformation("Closed");
            return base.StopAsync(token);
        }
    }
}