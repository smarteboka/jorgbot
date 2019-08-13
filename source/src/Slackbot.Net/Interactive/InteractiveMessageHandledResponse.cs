namespace Slackbot.Net
{
    public class InteractiveMessageHandledResponse
    {
        public string Text
        {
            get;
            set;
        }

        public bool Is_Ephemeral
        {
            get;
            set;
        }

        public bool Delete_Original
        {
            get;
            set;
        }
    }
}