using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fpl.Client;
using Slackbot.Net.Workers.Handlers;
using Slackbot.Net.Workers.Publishers;
using SlackConnector.Models;

namespace Smartbot.Utilities.Handlers
{
    public class FplHandler : IHandleMessages
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly IFplClient _fplClient;

        public FplHandler(IEnumerable<IPublisher> publishers, IFplClient fplClient)
        {
            _publishers = publishers;
            _fplClient = fplClient;
        }

        public Tuple<string, string> GetHelpDescription()
        {
            return new Tuple<string, string>("fpl", "Henter stillingen fra Smartasy Premier League");
        }

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var standings = await GetStandings();

            foreach (var p in _publishers)
            {
                await p.Publish(new Notification {Recipient = message.ChatHub.Id, Msg = standings});
            }

            return new HandleResponse(standings);
        }

        private async Task<string> GetStandings()
        {
            try
            {
                var scoreboard = await _fplClient.GetScoreBoard("89903");
                var bootstrap = await _fplClient.GetBootstrap();
                var standings = FplFormatter.GetStandings(scoreboard, bootstrap);
                return standings;
            }
            catch (Exception e)
            {
                return $"Oops: {e.Message}";
            }
        }

        public bool ShouldHandle(SlackMessage message)
        {
            return message.MentionsBot && message.Text.Contains("fpl");
        }

        public bool ShouldShowInHelp => true;
    }
}