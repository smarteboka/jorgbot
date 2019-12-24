using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Core.Integrations.SlackAPIExtensions;
using Slackbot.Net.Workers.Handlers;
using SlackConnector.Models;
using Smartbot.Utilities.Storage;
using Smartbot.Utilities.Storage.SlackUrls;
using ILogger = SlackConnector.Logging.ILogger;

namespace Smartbot.Utilities.Handlers
{
    public class UrlsSaveHandler : IHandleMessages
    {
        private readonly SlackMessagesStorage _storage;
        private readonly SlackTaskClientExtensions _client;
        private readonly ILogger<UrlsSaveHandler> _logger;

        public UrlsSaveHandler(SlackMessagesStorage storage, SlackTaskClientExtensions client, ILogger<UrlsSaveHandler> logger)
        {
            _storage = storage;
            _client = client;
            _logger = logger;
        }

        public bool ShouldShowInHelp => false;

        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("<all>", "Lagrer unna lenker");

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var urls = RegexHelper.FindUrls(message.Text);
            var permalink = await _client.GetPermalink(message.ChatHub.Id, message.Timestamp.ToString("N6"));

            foreach (var url in urls)
            {
                var cleansedUrl = UrlCleaner.CleanForTrackingQueryParams(url);
                cleansedUrl = cleansedUrl.TrimEnd('/');
                var slackMessageEntity = new SlackUrlEntity(Guid.NewGuid())
                {
                    Text = message.Text,
                    User = message.User.Name,
                    ChannelName = message.ChatHub.Name,
                    Url = cleansedUrl,
                    Raw = message.RawData,
                    SlackTimestamp = message.Timestamp.ToString("N6"),
                    Permalink = permalink
                };
                await _storage.Save(slackMessageEntity);
                _logger.LogInformation($"Saved url. {cleansedUrl}");
            }
            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            return RegexHelper.FindUrls(message.Text).Any();
        }
    }
}