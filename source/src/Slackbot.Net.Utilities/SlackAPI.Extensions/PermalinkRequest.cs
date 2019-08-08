namespace Slackbot.Net.Utilities.SlackAPI.Extensions
{
    public class PermalinkRequest
    {
        public string Channel
        {
            get;
            set;
        }

        public string Message_ts
        {
            get;
            set;
        }
    }

    public class PermalinkResponse
    {
        public string Permalink
        {
            get;
            set;
        }
    }
}