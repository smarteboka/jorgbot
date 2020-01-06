using System;
using System.Runtime.Serialization;

namespace SlackConnector.Exceptions
{
    public class CommunicationException : Exception
    {
        public CommunicationException(string message) : base(message)
        { }
    }
}