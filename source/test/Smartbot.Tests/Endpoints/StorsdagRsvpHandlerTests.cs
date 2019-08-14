using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Endpoints.Interactive;
using Smartbot.Utilities.Interactive;
using Xunit;

namespace Smartbot.Tests.Endpoints
{
    public class StorsdagRsvpHandlerTests
    {
        [Fact]
        public async Task HandlesDeltar()
        {
            var mockResponder = A.Fake<IRespond>();
            A.CallTo(() => mockResponder.Respond(A<string>._, A<string>._)).Returns(new RespondResult { Success = true });

            var responseHandler = new StorsdagRsvpResponseHandler(new DummyLogger(), mockResponder);
            var deltarMessage = IncomingWith("deltar");
            var res = await responseHandler.RespondToSlackInteractivePayload(deltarMessage);
            Assert.True((res as RsvpResult).Attending);
        }

        [Fact]
        public async Task HandlesDeltarIkke()
        {
            var mockResponder = A.Fake<IRespond>();
            A.CallTo(() => mockResponder.Respond(A<string>._, A<string>._)).Returns(new RespondResult { Success = true });

            var responseHandler = new StorsdagRsvpResponseHandler(new DummyLogger(), mockResponder);
            var deltarMessage = IncomingWith("deltar-ikke");
            var res = await responseHandler.RespondToSlackInteractivePayload(deltarMessage);
            Assert.False((res as RsvpResult).Attending);
        }


        private static IncomingInteractiveMessage IncomingWith(string value)
        {
            var message = new IncomingInteractiveMessage
            {
                Actions = new[]
                {
                    new ValueBlock
                    {
                        value = value
                    }
                }
            };
            return message;
        }
    }

    internal class DummyLogger : ILogger<StorsdagRsvpResponseHandler>
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