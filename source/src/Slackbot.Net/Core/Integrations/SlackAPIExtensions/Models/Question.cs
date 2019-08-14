using System.Collections.Generic;

namespace Slackbot.Net.Core.Integrations.SlackAPIExtensions.Models
{
    public class Question
    {
        public string Message
        {
            get;
            set;
        }

        public string Channel
        {
            get;
            set;
        }

        public IEnumerable<QuestionOption> Options
        {
            get;
            set;
        }

        public string Botname
        {
            get;
            set;
        }
    }
}