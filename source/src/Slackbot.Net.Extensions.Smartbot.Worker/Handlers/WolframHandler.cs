using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Slackbot.Net.Endpoints.Abstractions;
using Slackbot.Net.Endpoints.Models.Events;
using Slackbot.Net.SlackClients.Http;
using WolframAlphaNet;

namespace Smartbot.Utilities.Handlers
{
    public class WolframHandler : IHandleAppMentions
    {
        private readonly IOptions<WulframOptions> _options;
        private readonly ISlackClient _client;

        public WolframHandler(IOptions<WulframOptions> options, ISlackClient client)
        {
            _options = options;
            _client = client;
        }

        public (string, string) GetHelpDescription() => ("wolf", "Gir deg Wolfram Alpha svar");

        public async Task<EventHandledResponse> Handle(EventMetaData data, AppMentionEvent message)
        {
            
            var messageText = message.Text.Replace($"wolf ", "");

            var wolfram = new WolframAlpha(_options.Value.WulframAlphaAppId);
            var results = wolfram.Query(messageText);
            var text = "";
            if (results != null)
            {
                foreach (var pod in results.Pods)
                {
                    if (pod.SubPods != null)
                    {
                        foreach (var subPod in pod.SubPods)
                        {
                            if(!string.IsNullOrEmpty(subPod.Title))
                                text += $"•{subPod.Title}\n";
                            
                            text += $"{subPod.Plaintext} \n";
                            text += $"\n";
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(text))
                text = "Wulf aner ikke 🐺";

            await _client.ChatPostMessage(message.Channel, text);

            return new EventHandledResponse("OK");
        }

        public bool ShouldHandle(AppMentionEvent message) => Contains(message.Text, "wolf");

        private static bool Contains(string haystack, params string[] needles) => needles.Any(s => haystack.Contains(s, StringComparison.InvariantCultureIgnoreCase));
    }

    public class WulframOptions
    {
        public string WulframAlphaAppId { get; set; }
    }
}