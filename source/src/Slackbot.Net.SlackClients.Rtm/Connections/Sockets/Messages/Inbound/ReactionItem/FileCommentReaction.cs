using Newtonsoft.Json;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Sockets.Messages.Inbound.ReactionItem
{
    internal class FileCommentReaction : IReactionItem
    {
        [JsonProperty("file")]
        public string File { get; set; }

        [JsonProperty("file_comment")]
        public string FileComment { get; set; }
    }
}
