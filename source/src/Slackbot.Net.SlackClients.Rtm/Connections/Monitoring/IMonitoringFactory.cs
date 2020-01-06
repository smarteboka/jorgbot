namespace Slackbot.Net.SlackClients.Rtm.Connections.Monitoring
{
    internal interface IMonitoringFactory
    {
        IPingPongMonitor CreatePingPongMonitor();
    }
}