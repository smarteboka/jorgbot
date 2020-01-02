using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Publishers;
using SlackConnector.Models;

namespace Smartbot.Utilities.Handlers
{
    public class RandomSmartingHandler : IHandleMessages
    {
        private readonly Smartinger _smartinger;
        private readonly IEnumerable<IPublisher> _publishers;

        public RandomSmartingHandler(Smartinger smartinger, IEnumerable<IPublisher> publishers)
        {
            _smartinger = smartinger;
            _publishers = publishers;
        }

        public bool ShouldShowInHelp => true;


        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("random, tilfeldig", "Gir deg en tilfeldig smarting");

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var index = new Random().Next(_smartinger.Smartingene.Count);
            var randomSmarting = _smartinger.Smartingene[index];
            foreach (var publisher in _publishers)
            {
                await publisher.Publish(new Notification
                {
                    Msg = randomSmarting.Name,
                    Recipient = message.ChatHub.Id
                });
            }

            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            return message.MentionsBot && Contains(message.Text, "random", "tilfeldig");
        }

        private static bool Contains(string haystack, params string[] needles) => needles.Any(s => haystack.Contains(s, StringComparison.InvariantCultureIgnoreCase));
    }
}