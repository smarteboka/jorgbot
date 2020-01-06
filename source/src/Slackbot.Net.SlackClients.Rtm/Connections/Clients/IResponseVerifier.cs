using Slackbot.Net.SlackClients.Rtm.Connections.Responses;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Clients
{
    internal interface IResponseVerifier
    {
        void VerifyResponse(StandardResponse response);
    }
}