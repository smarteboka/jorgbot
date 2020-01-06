using Newtonsoft.Json;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound.ReactionItem
{
    internal class MessageReaction : IReactionItem
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }
    }
}
