using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Slackbot.Net.Abstractions.Hosting;
using Slackbot.Net.SlackClients.Http.Configurations.Options;

namespace Smartbot
{
    public class InMemTokenStore : ITokenStore
    {
        private string _token;

        public InMemTokenStore(IOptions<BotTokenClientOptions> options)
        {
            _token = options.Value.BotToken;
        }
        
        public Task<IEnumerable<string>> GetTokens()
        {
            var strings = new List<string>{ _token };
            return Task.FromResult(strings.AsEnumerable());
        }

        public Task<string> GetTokenByTeamId(string teamId)
        {
            return Task.FromResult(_token);
        }

        public Task Delete(string token)
        {
            return Task.CompletedTask;
        }
    }
}