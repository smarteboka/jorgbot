using System;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Handlers.Models.Rtm.MessageReceived;
using Slackbot.Net.SlackClients.Http;
using Slackbot.Net.SlackClients.Http.Models.Requests.ChatPostMessage;

namespace Smartbot.Utilities.Handlers
{
    public class TellHandler : IHandleMessages
    {
        private readonly ISlackClient _slackClient;

        public TellHandler(ISlackClient slackClient)
        {
            _slackClient = slackClient;
        }

        public bool ShouldShowInHelp => true;

        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("tell <slack-user> <what>", "Be smartbot om å fortelle en smarting noe på DM");

        /// <summary>
        /// @smartbot tell @john bull hop stuff
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var args = message.Text.Split(' ');
            var recipient = message.ChatHub.Id;
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

            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            var sharedUrls = message.Text.Contains("tell", StringComparison.InvariantCultureIgnoreCase);
            return message.MentionsBot && sharedUrls;        
        }
    }
}