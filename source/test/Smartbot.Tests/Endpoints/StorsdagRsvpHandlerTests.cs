using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Endpoints.Interactive;
using Smartbot.Utilities.Storage.Events;
using Smartbot.Utilities.Storsdager.Interactive;
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

            var mockStorage = A.Fake<IInvitationsStorage>();
            var responseHandler = new EventRsvpResponseHandler(new DummyLogger(), mockResponder, mockStorage);
            var deltarMessage = IncomingWith(RsvpValues.Attending);
            var res = await responseHandler.RespondToSlackInteractivePayload(deltarMessage);
            Assert.Equal(RsvpValues.Attending,(res as RsvpResult).Rsvp);
        }

        [Fact]
        public async Task HandlesDeltarKanskje()
        {
            var mockResponder = A.Fake<IRespond>();
            A.CallTo(() => mockResponder.Respond(A<string>._, A<string>._)).Returns(new RespondResult { Success = true });
            var mockStorage = A.Fake<IInvitationsStorage>();

            var responseHandler = new EventRsvpResponseHandler(new DummyLogger(), mockResponder, mockStorage);
            var deltarMessage = IncomingWith(RsvpValues.Maybe);
            var res = await responseHandler.RespondToSlackInteractivePayload(deltarMessage);
            Assert.Equal(RsvpValues.Maybe,(res as RsvpResult).Rsvp);
        }

        [Fact]
        public async Task HandlesDeltarIkke()
        {
            var mockResponder = A.Fake<IRespond>();
            A.CallTo(() => mockResponder.Respond(A<string>._, A<string>._)).Returns(new RespondResult { Success = true });
            var mockStorage = A.Fake<IInvitationsStorage>();

            var responseHandler = new EventRsvpResponseHandler(new DummyLogger(), mockResponder, mockStorage);
            var deltarMessage = IncomingWith(RsvpValues.NotAttending);
            var res = await responseHandler.RespondToSlackInteractivePayload(deltarMessage);
            Assert.Equal(RsvpValues.NotAttending,(res as RsvpResult).Rsvp);
        }


        private static IncomingInteractiveMessage IncomingWith(string value)
        {
            var message = new IncomingInteractiveMessage
            {
                Actions = new[]
                {
                    new ValueBlock
                    {
                        block_id = "someblockid",
                        value = value
                    }
                }
            };
            return message;
        }
    }

    internal class DummyLogger : ILogger<EventRsvpResponseHandler>
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