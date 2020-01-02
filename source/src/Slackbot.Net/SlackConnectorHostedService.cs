using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Connections;

namespace Slackbot.Net
{
    internal class SlackConnectorHostedService : BackgroundService
    {
        private readonly SlackConnectionSetup _setup;
        private readonly ILogger<SlackConnectorHostedService> _logger;

        public SlackConnectorHostedService(SlackConnectionSetup setup, ILogger<SlackConnectorHostedService> logger)
        {
            _setup = setup;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Running");
            await _setup.Connect();
        }
    }
}