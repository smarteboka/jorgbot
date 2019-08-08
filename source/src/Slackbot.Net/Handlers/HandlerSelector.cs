using System.Collections.Generic;
using System.Linq;
using SlackConnector.Models;

namespace Slackbot.Net.Strategies
{
    public class HandlerSelector
    {
        private readonly IEnumerable<IHandleMessages> _handlers;

        public HandlerSelector(IEnumerable<IHandleMessages> handlers)
        {
            _handlers = handlers;
        }

        public IEnumerable<IHandleMessages> SelectHandler(SlackMessage message)
        {
            var matchingHandlers = _handlers.Where(s => s.ShouldHandle(message));
            foreach (var handler in matchingHandlers)
            {
                yield return handler;
            }

            yield return new NoOpHandler();
        }
    }
}