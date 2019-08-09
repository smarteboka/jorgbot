using System;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Strategies;
using Slackbot.Net.Utilities;
using Slackbot.Net.Utilities.SlackAPI.Extensions;
using SlackConnector.Models;
using Smartbot.Utilities.Storage;

namespace Smartbot.Utilities.Handlers
{
    public class UrlsSaveHandler : IHandleMessages
    {
        private readonly SlackMessagesStorage _storage;
        private readonly ISlackClient _client;

        public UrlsSaveHandler(SlackMessagesStorage storage, ISlackClient client)
        {
            _storage = storage;
            _client = client;
        }

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
            }
            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            return RegexHelper.FindUrls(message.Text).Any();
        }
    }
}