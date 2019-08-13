using System;
using Microsoft.Extensions.Logging;

namespace Slackbot.Net.Tests
{
    internal class DummyLogger : ILogger<InteractiveResponseHandler>
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

        }

        public bool IsEnabled(LogLevel logLevel) => false;


        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}