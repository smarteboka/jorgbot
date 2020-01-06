using Slackbot.Net.SlackClients.Rtm.Connections.Responses;
using Slackbot.Net.SlackClients.Rtm.Exceptions;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Clients
{
    internal class ResponseVerifier : IResponseVerifier
    {
        public void VerifyResponse(StandardResponse response)
        {
            if (!response.Ok)
            {
                throw new CommunicationException($"Error occured while posting message '{response.Error}'");
            }
        }
    }
}