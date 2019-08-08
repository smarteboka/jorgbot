using System.ComponentModel.DataAnnotations;

namespace Slackbot.Net.Hosting
{
    public class CronOptions
    {
        [Required]
        public string Cron
        {
            get;
            set;
        }
    }
}