namespace Slackbot.Net.SlackClients.Models.Responses.SearchMessages
{
    public class SearchMessagesResponse : Response
    {
        public SearchResponseMessagesContainer Messages;
    }

    public class SearchResponseMessagesContainer
    {
        public ContextMessage[] matches;
    }

    public class ContextMessage : Message
    {
        public Message previous_2;
        public Message previous;
        public Message next;
        public Message next_2;
    }
    
    public class Error
    {
        public int code;
        public string msg;
    }
    
    public class SlackSocketMessage
    {
        public bool ok = true;
        public int id;
        public int reply_to;
        public string type;
        public string subtype;
        public Error error;
    }
    
    public class Reaction
    {
        public string Name { get; set; }
        public string Channel { get; set; }
        public string Timestamp { get; set; }
    }

    public class Message : SlackSocketMessage
    {
        public Channel channel;
        public string ts; //epoch
        public string user;
        public string username;
        public string text;
        public bool is_starred;
        public string permalink;
        public Reaction[] reactions;
    }

    public class Channel
    {
        public string name;
        public string id;
    }
}