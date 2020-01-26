using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Handlers.Models.Rtm.MessageReceived;
using Slackbot.Net.Abstractions.Publishers;
using WolframAlphaNet;

namespace Smartbot.Utilities.Handlers
{
    public class WolframHandler : IHandleMessages
    {
        private readonly IOptions<WulframOptions> _options;
        private readonly IEnumerable<IPublisher> _publishers;

        public WolframHandler(IOptions<WulframOptions> options, IEnumerable<IPublisher> publishers)
        {
            _options = options;
            _publishers = publishers;
        }

        public bool ShouldShowInHelp => true;


        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("wolf", "Gir deg Wolfram Alpha svar");

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var botDetails = message.Bot;
            var messageText = message.Text.Replace($"<@{botDetails.Id}> wolf ", "");

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
            
            foreach (var publisher in _publishers)
            {
                await publisher.Publish(new Notification
                {
                    Msg = text,
                    Recipient = message.ChatHub.Id
                });
            }

            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            return message.MentionsBot && Contains(message.Text, "wolf");
        }

        private static bool Contains(string haystack, params string[] needles) => needles.Any(s => haystack.Contains(s, StringComparison.InvariantCultureIgnoreCase));
    }

    public class WulframOptions
    {
        public string WulframAlphaAppId { get; set; }
    }
}