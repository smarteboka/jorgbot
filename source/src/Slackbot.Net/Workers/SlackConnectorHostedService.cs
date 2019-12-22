using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net.Workers.Handlers;
using SlackConnector;

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

        // public SlackConnectorHostedService(ISlackConnector slackConnector,
        //     IOptions<SlackOptions> options,
        //     ILogger<SlackConnectorHostedService> logger,
        //     HandlerSelector handlerSelector)
        // {
        //     _slackConnector = slackConnector;
        //     _logger = logger;
        //     _handlerSelector = handlerSelector;
        //     _config = options.Value;
        // }

        public SlackConnectorHostedService(ISlackConnection connection)
        {
            _connection = connection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
             
            // _connection = await _slackConnector.Connect(_config.Slackbot_SlackApiKey_BotUser);
            // _connection.OnMessageReceived += _handlerSelector.HandleIncomingMessage;
            //
            // _connection.OnDisconnect += () =>
            // {
            //     _connected = false;
            //     _logger.LogInformation("Disconnecting");
            // };
            // if (_connection.IsConnected)
            // {
            //     _logger.LogInformation("Connected");
            //     _connected = true;
            // }
            //
            // while (!stoppingToken.IsCancellationRequested && !_connected)
            // {
            //
            // }
        }

        public override async Task StopAsync(CancellationToken token)
        {
            _logger.LogInformation("Closing");
            await _connection.Close();
            _logger.LogInformation("Closed");
            await base.StopAsync(token);
        }
    }
}