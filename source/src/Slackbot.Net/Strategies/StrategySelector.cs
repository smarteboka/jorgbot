using System.Collections.Generic;
using System.Linq;
using SlackConnector.Models;

namespace Slackbot.Net.Strategies
{
    public class StrategySelector
    {
        private readonly IEnumerable<IReplyStrategy> _strategies;

        public StrategySelector(IEnumerable<IReplyStrategy> strategies)
        {
            _strategies = strategies;
        }

        public IEnumerable<IReplyStrategy> DirectToStrategy(SlackMessage message)
        {
            var matches = _strategies.Where(s => s.ShouldExecute(message));
            foreach (var matchingStrategy in matches)
            {
                yield return matchingStrategy;
            }

            yield return new DoNothingStrategy();
        }
    }
}