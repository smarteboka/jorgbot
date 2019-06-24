using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SlackConnector;
using SlackConnector.Models;

namespace Oldbot.ConsoleApp
{
    internal class OldbotHostedService : BackgroundService
    {
        private readonly ISlackConnector _noobotCore;
        private readonly OldnessValidator _validator;
        private readonly ILogger<OldbotHostedService> _logger;
        private readonly OldbotConfig _config;

        public OldbotHostedService(ISlackConnector noobotCore, IOptions<OldbotConfig> options, OldnessValidator validator, ILogger<OldbotHostedService> logger)
        {
            _noobotCore = noobotCore;
            _validator = validator;
            _logger = logger;
            _config = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = await _noobotCore.Connect(_config.SlackApiKeyBotUser);
            connection.OnMessageReceived += ConnectionOnOnMessageReceived;
        }

        private Task ConnectionOnOnMessageReceived(SlackMessage message)
        {
            return _validator.Validate(message);
        }
    }
}