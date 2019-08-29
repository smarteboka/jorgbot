using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net.Workers.Handlers;
using SlackConnector;
using SlackConnector.Models;

namespace Slackbot.Net.Workers
{
    internal class SlackConnectorHostedService : BackgroundService
    {
        private readonly ISlackConnector _slackConnector;
        private readonly ILogger<SlackConnectorHostedService> _logger;
        private readonly HandlerSelector _handlerSelector;
        private readonly HelpHandler _helpHandler;
        private readonly SlackOptions _config;
        private ISlackConnection _connection;
        private bool _connected;

        public SlackConnectorHostedService(ISlackConnector slackConnector,
            IOptions<SlackOptions> options,
            ILogger<SlackConnectorHostedService> logger,
            HandlerSelector handlerSelector, HelpHandler helpHandler)
        {
            _slackConnector = slackConnector;
            _logger = logger;
            _handlerSelector = handlerSelector;
            _helpHandler = helpHandler;
            _config = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && !_connected)
            {
                _logger.LogInformation("Connecting");
                _connection = await _slackConnector.Connect(_config.Slackbot_SlackApiKey_BotUser);
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
            if (_helpHandler.ShouldHandle(message))
            {
                await _helpHandler.Handle(message);
                return;
            }

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