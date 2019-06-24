using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackConnector;
using SlackConnector.Models;
using Smartbot.Publishers.Slack;

namespace Smartbot.HostedServices
{
    internal class OldbotHostedService : BackgroundService
    {
        private readonly ISlackConnector _noobotCore;
        private readonly OldnessValidator _validator;
        private readonly ILogger<OldbotHostedService> _logger;
        private readonly SlackOptions _config;
        private ISlackConnection _connection;
        private bool _connected;

        public OldbotHostedService(ISlackConnector noobotCore, IOptions<SlackOptions> options, OldnessValidator validator, ILogger<OldbotHostedService> logger)
        {
            _noobotCore = noobotCore;
            _validator = validator;
            _logger = logger;
            _config = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && !_connected)
            {
                _logger.LogInformation("Connecting");
                _connection = await _noobotCore.Connect(_config.SmartBot_SlackApiKey_BotUser);
                _connection.OnMessageReceived += ConnectionOnOnMessageReceived;
                
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

        public override Task StopAsync(CancellationToken token)
        {
            _logger.LogInformation("Closing");
             _connection.Close().GetAwaiter().GetResult();
            _logger.LogInformation("Closed");
            return base.StopAsync(token);
        }

        private Task ConnectionOnOnMessageReceived(SlackMessage message)
        {
            return _validator.Validate(message);
        }
    }
}