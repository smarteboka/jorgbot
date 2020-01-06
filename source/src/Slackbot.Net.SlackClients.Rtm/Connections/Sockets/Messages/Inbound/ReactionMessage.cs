using Newtonsoft.Json;
using Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound.ReactionItem;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound
{
    internal class ReactionMessage : InboundMessage
    {
        public ReactionMessage()
        {
        }
        
        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("reaction")]
        public string Reaction { get; set; }
      
        [JsonProperty("event_ts")]
        public double Timestamp { get; set; }

        public IReactionItem ReactingTo { get; set; }

        [JsonProperty("item_user")]
        public string ReactingToUser { get; set; }
    }
}
