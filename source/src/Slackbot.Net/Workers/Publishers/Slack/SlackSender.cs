using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackAPI;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions;

namespace Slackbot.Net.Workers.Publishers.Slack
{
    public class SlackSender
    {
        private readonly ILogger<SlackSender> _logger;
        private readonly SlackTaskClientExtensions _slackAppClient;
        private readonly SlackTaskClientExtensions _slackBotUserClient;

        public SlackSender(SlackTaskClientExtensions client)
        {
            _slackBotUserClient = client;
        }
    }
}