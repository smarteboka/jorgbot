using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackConnector;
using SlackConnector.Models;
using Smartbot.HostedServices.Strategies;
using Smartbot.Publishers.Slack;

namespace Smartbot.HostedServices
{
    internal class RealTimeBotHostedService : BackgroundService
    {
        private readonly ISlackConnector _noobotCore;
        private readonly OldnessValidator _validator;
        private readonly ILogger<RealTimeBotHostedService> _logger;
        private readonly StrategySelector _strategySelector;
        private readonly SlackOptions _config;
        private ISlackConnection _connection;
        private bool _connected;

        public RealTimeBotHostedService(ISlackConnector noobotCore, IOptions<SlackOptions> options, OldnessValidator validator, ILogger<RealTimeBotHostedService> logger, StrategySelector strategySelector)
        {
            _noobotCore = noobotCore;
            _validator = validator;
            _logger = logger;
            _strategySelector = strategySelector;
            _config = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && !_connected)
            {
                _logger.LogInformation("Connecting");
                _connection = await _noobotCore.Connect(_config.SmartBot_SlackApiKey_BotUser);
                _connection.OnMessageReceived += ValidateOldness;
                _connection.OnMessageReceived += ReplyToCommands;
                
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

        private async Task ReplyToCommands(SlackMessage message)
        {
            var strategies = _strategySelector.DirectToStrategy(message);
            foreach (var strategy in strategies)
            {
                await strategy.Handle(message);
            }
        }

        public override Task StopAsync(CancellationToken token)
        {
            _logger.LogInformation("Closing");
             _connection.Close().GetAwaiter().GetResult();
            _logger.LogInformation("Closed");
            return base.StopAsync(token);
        }

        private Task ValidateOldness(SlackMessage message)
        {
            return _validator.Validate(message);
        }
    }
}