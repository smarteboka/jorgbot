using System.ComponentModel.DataAnnotations;

namespace Slackbot.Net.Publishers.Slack
{
    public class SlackOptions
    {
        [Required]
        public string Slackbot_SlackApiKey_SlackApp
        {
            get;
            set;
        }

        [Required]
        public string Slackbot_SlackApiKey_BotUser
        {
            get;
            set;
        }
    }

}