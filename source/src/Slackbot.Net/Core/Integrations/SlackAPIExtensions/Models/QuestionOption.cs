namespace Slackbot.Net.Core.Integrations.SlackAPIExtensions.Models
{
    public class QuestionOption
    {

        public string Text;

        public string ActionId
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public QuestionConfirmation Confirmation
        {
            get;
            set;
        }
    }
}