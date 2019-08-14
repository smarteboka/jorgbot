namespace Slackbot.Net.Workers.Handlers
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