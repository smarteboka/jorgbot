namespace Slackbot.Net.SlackClients.Rtm.Connections.Monitoring
{
    internal class MonitoringFactory : IMonitoringFactory
    {
        public IPingPongMonitor CreatePingPongMonitor()
        {
            return new PingPongMonitor(new Timer(), new DateTimeKeeper());
        }
    }
}