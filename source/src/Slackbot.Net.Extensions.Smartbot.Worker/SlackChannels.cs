using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Smartbot.Utilities
{
    public class SlackChannels
    {
        public SlackChannels(IHostingEnvironment env, ILogger<SlackChannels> logger)
        {
            if (env.IsDevelopment())
            {
                logger.LogInformation("Development. Using testchannel");
                BursdagerChannel = TestChannelId;
                SmartebokaChannel = TestChannelId;
                JorgChannel = TestChannelId;
                TestChannel = TestChannelId;
                StorsdagChannel = TestChannelId;
            }
            else
            {
                logger.LogInformation("Production. Using prod channels");
                BursdagerChannel = BursdagerChannelId;
                SmartebokaChannel = SmartebokaChannelId;
                JorgChannel = JorgChannelId;
                TestChannel = TestChannelId;
                StorsdagChannel = StorsdagChannelId;

            }
        }

        /// <summary>
        /// https://smarteboka.slack.com/messages/CGY1XJRM1/details/
        /// </summary>
        public const string TestChannelId = "CGY1XJRM1";
        public const string BursdagerChannelId = "CK1TE2NN6";
        public const string SmartebokaChannelId = "C0EC3DG5N";
        public const string JorgChannelId = "CKDDD3MLM";
        public const string StorsdagChannelId = "CK2A5K12N";

        public string BursdagerChannel
        {
            get;
        }

        public string SmartebokaChannel
        {
            get;
        }

        public string JorgChannel
        {
            get;
        }

        public string TestChannel
        {
            get;
        }

        public string StorsdagChannel
        {
            get;
        }
    }
}