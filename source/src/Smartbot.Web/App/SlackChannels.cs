namespace Smartbot.Web.App;

public class SlackChannels
{
    public SlackChannels(IHostEnvironment env, ILogger<SlackChannels> logger)
    {
        if (env.IsDevelopment())
        {
            logger.LogInformation("Development. Using testchannel");
            SmartebokaChannel = TestChannelId;
        }
        else
        {
            logger.LogInformation("Production. Using prod channels");
            SmartebokaChannel = SmartebokaChannelId;
        }
    }

    /// <summary>
    /// https://smarteboka.slack.com/messages/CGY1XJRM1/details/
    /// </summary>
    public const string TestChannelId = "CGY1XJRM1";
    public const string SmartebokaChannelId = "C0EC3DG5N";

    public string SmartebokaChannel
    {
        get;
    }

}