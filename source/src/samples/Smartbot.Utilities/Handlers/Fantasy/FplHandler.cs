using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fpl.Client.Abstractions;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Publishers;
using SlackConnector.Models;

namespace Smartbot.Utilities.Handlers
{
    public class FplHandler : IHandleMessages
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly IGameweekClient _gameweekClient;
        private readonly ILeagueClient _leagueClient;

        public FplHandler(IEnumerable<IPublisher> publishers, IGameweekClient gameweekClient, ILeagueClient leagueClient)
        {
            _publishers = publishers;
            _gameweekClient = gameweekClient;
            _leagueClient = leagueClient;
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
                var scoreboard = await _leagueClient.GetClassicLeague(89903);
                var gameweeks = await _gameweekClient.GetGameweeks();
                var standings = FplFormatter.GetStandings(scoreboard, gameweeks);
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