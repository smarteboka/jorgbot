using System;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Endpoints.Abstractions;
using Slackbot.Net.Endpoints.Models.Events;
using Slackbot.Net.SlackClients.Http;
using Slackbot.Net.SlackClients.Http.Models.Requests.ChatPostMessage;

namespace Smartbot.Utilities.Handlers
{
    public class TellHandler : IHandleAppMentions
    {
        private readonly ISlackClient _slackClient;

        public TellHandler(ISlackClient slackClient)
        {
            _slackClient = slackClient;
        }

        public (string, string) GetHelpDescription() => ("tell <slack-user> <what>", "Be smartbot om å fortelle en smarting noe på DM");

        /// <summary>
        /// @smartbot tell @john bull hop stuff
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<EventHandledResponse> Handle(EventMetaData data, AppMentionEvent message)
        {
            var args = message.Text.Split(' ');
            var recipient = message.Channel;
            var content = "Psst";
            
            if (args.Length == 2)
            {
                content = "lol, du må gi meg et navn å sende til..`@smartbot tell @nash du er en tufs`";
            }

            if (args.Length == 3)
            {
                content = "lol, du må gi meg noe innhold å sende! `@smartbot tell @nash du er en tufs`";
            }
            
            if(args.Length > 3)
            {
                
                var parsedName = args[2];
                
                var users = await _slackClient.UsersList();
                var usr = users.Members.FirstOrDefault(u => $"<@{u.Id}>" == parsedName);
                
                var channels = await _slackClient.ConversationsListPublicChannels();
                var channel = channels.Channels.FirstOrDefault(c => string.Equals($"<#{c.Id}|{c.Name}>",parsedName, StringComparison.InvariantCultureIgnoreCase));

                content = string.Join(" ", args[3..args.Length]);
                
                if (usr != null)
                {
                    recipient = usr.Id;
                }
                else if(channel != null)
                {
                    recipient = channel.Id;
                }
                else
                {
                    content = $"Kunne ikke sende noe til `{parsedName}` :/";
                }
            }

            var res = await _slackClient.ChatPostMessage(new ChatPostMessageRequest
            {
                Channel = recipient,
                as_user = "true",
                Text = content
            });

            return new EventHandledResponse("OK");
        }

        public bool ShouldHandle(AppMentionEvent message) => message.Text.StartsWith("cmd") && message.Text.Contains("tell", StringComparison.InvariantCultureIgnoreCase);
    }
}
