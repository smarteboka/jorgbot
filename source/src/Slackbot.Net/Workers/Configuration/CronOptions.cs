using System.ComponentModel.DataAnnotations;
using Slackbot.Net.Validations;
using Slackbot.Net.Workers.Validations;

namespace Slackbot.Net.Workers.Configuration
{
    public class CronOptions
    {
        public CronOptions()
        {
            // Defaults to Oslo time
            if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
            {
                TimeZoneId = "Europe/Oslo";
            }
            else
            {
                TimeZoneId = "Central European Standard Time";
            }
        }

        [Required]
        public string Cron
        {
            get;
            set;
        }


        /// <summary>
        /// Default: Europe/oslo
        /// </summary>
        [RequiredValidTimeZone]
        public string TimeZoneId
        {
            get;
            set;
        }
    }
}