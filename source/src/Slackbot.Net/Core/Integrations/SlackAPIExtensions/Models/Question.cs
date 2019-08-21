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

        public string Recipient
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

        // block_id
        public string QuestionId
        {
            get;
            set;
        }

        public string Image
        {
            get;
            set;
        }
    }
}