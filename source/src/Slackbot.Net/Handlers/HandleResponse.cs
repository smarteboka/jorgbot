namespace Slackbot.Net.Strategies
{
    public class HandleResponse
    {
        public HandleResponse(string message)
        {
            HandledMessage = message;
        }

        public string HandledMessage
        {
            get;
            set;
        }
    }
}