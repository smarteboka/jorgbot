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

        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("tell <slack-user> <what>", "Siste delte lenker");

        /// <summary>
        /// @smartbot tell @john bull hop stuff
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var args = message.Text.Split(' ');
            var recipient = message.User.Id;
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
                var users = await _slackClient.UsersList();
                var parsedName = args[2];
                var usr = users.Members.FirstOrDefault(u => $"<@{u.Id}>" == parsedName);
                if (usr != null)
                {
                    recipient = usr.Id;
                    var end = args.Length;
                    var restOfit = string.Join(" ", args[3..end]);
                    content = restOfit;
                }
                else
                {
                    content = $"lol, fant ikke {parsedName}";
                }
            }

            var res = await _slackClient.ChatPostMessage(new ChatPostMessageRequest
            {
                Channel = recipient,
                as_user = "true",
                Text = content
            });

            if (!res.Ok)
            {
                await _slackClient.ChatPostMessage(new ChatPostMessageRequest
                {
                    Channel = message.ChatHub.Id,
                    as_user = "true",
                    Text = "Klarte ikke :/ " + res.Error
                });
            }
          
            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            var sharedUrls = message.Text.Contains("tell", StringComparison.InvariantCultureIgnoreCase);
            return message.MentionsBot && sharedUrls;        
        }
    }
}